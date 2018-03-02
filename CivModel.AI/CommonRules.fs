namespace CivModel.AI

open System
open System.Linq
open CivModel

open AIFuzzyModule

type CommonRules(player : CivModel.Player) =
    let rules = fuzzy.CreateRule <| RuleString.Split([| "\r"; "\n" |], StringSplitOptions.RemoveEmptyEntries)

    let DoBuildInterior (getProfit : InteriorBuildingConstants -> float) (x : float32) =
        let getPoint (p : IInteriorBuildingProductionFactory) =
            let profit = getProfit p.Constants |> float32
            if profit > 0.f then
                rules |> NeedProduction.SetValue x
                rules |> Profit.SetValue profit
                rules |> ProductionLaborCost.SetValue (float32 p.TotalLaborCost)
                rules |> ProductionGoldCost.SetValue (float32 p.TotalGoldCost)
                rules |> BuildProduction.GetValue
            else
                -infinityf
        let (factory, point) =
            player.AvailableProduction
            |> Enumerable.OfType<IInteriorBuildingProductionFactory>
            |> Seq.map (fun p -> (p, getPoint p)) |> Seq.maxBy snd
        if point > 0.f then
            player.Production.AddLast(factory.Create(player)) |> ignore
        else
            ()

    let isProductionFull() =
        player.EstimateResourceInputs()
        player.GoldNetIncome <= 0.0 || player.Labor <= player.EstimatedUsedLabor

    let fuzzyActionList = [
        NeedLaboratory, DoBuildInterior (fun c -> c.ResearchIncome), isProductionFull;
        NeedFactory, DoBuildInterior (fun c -> c.ProvidedLabor), isProductionFull
    ]

    let setFuzzyInput() =
        rules |> Research.SetValue (float32 player.Research)
        rules |> Labor.SetValue (float32 player.Labor)

    let rec doFuzzyAction (space : (FuzzyVariable * (float32 -> unit) * (unit -> bool)) list) =
        seq {
            if not space.IsEmpty then
                setFuzzyInput()
                let (k, v, d, x) =
                    space
                    |> List.map (fun (k, v, d) -> (k, v, d, rules |> k.GetValue))
                    |> List.maxBy (fun (_, _, _, x) -> x)
                if x > 0.f then
                    if d() then
                        yield! doFuzzyAction (space |> List.filter (fun (k', _, _) -> k <> k'))
                    else
                        yield v x
                        yield! doFuzzyAction space
                else
                    ()
            else
                ()
        }

    member this.DoFuzzyAction() = doFuzzyAction fuzzyActionList

