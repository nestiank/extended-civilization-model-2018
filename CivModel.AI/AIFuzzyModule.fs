namespace CivModel.AI

open System
open System.Linq
open CivModel

module AIFuzzyModule =
    let fuzzy = FuzzySystem()

    let ResearchSets = fuzzy.CreateSetsByLevel "Research" [ "Low"; "Normal"; "High" ] 1 [ -infinityf; 100.f; 200.f; 300.f; infinityf ]
    let Research = fuzzy.CreateVariable "Research" 0.f infinityf ResearchSets

    let LaborSets = fuzzy.CreateSetsByLevel "Labor" [ "Low"; "Normal"; "High" ] 1 [ -infinityf; 100.f; 200.f; 300.f; infinityf ]
    let Labor = fuzzy.CreateVariable "Labor" 0.f infinityf LaborSets

    let CostSets = fuzzy.CreateSetsByLevel "Cost" [ "Low"; "Normal"; "High" ] 2 [ -infinityf; 5.f; 10.f; 15.f; 20.f; 25.f; infinityf ]
    let ProductionLaborCost = fuzzy.CreateVariable "ProductionLaborCost" 0.f infinityf CostSets
    let ProductionGoldCost = fuzzy.CreateVariable "ProductionGoldCost" 0.f infinityf CostSets

    let ProfitSets = fuzzy.CreateSetsByLevel "Profit" [ "Low"; "Normal"; "High" ] 1 [ -infinityf; 4.f; 8.f; 12.f; infinityf ]
    let Profit = fuzzy.CreateVariable "Profit" 0.f infinityf ProfitSets

    let ResultSets = fuzzy.CreateSetsByLevel "" [ "VeryLow"; "Low"; "Normal"; "High"; "VeryHigh" ] 1 [ -infinityf; -0.8f; -0.6f; -0.2f; 0.2f; 0.8f; infinityf ]
    let NeedLaboratory = fuzzy.CreateVariable "NeedLaboratory" -1.f 1.f ResultSets
    let NeedFactory = fuzzy.CreateVariable "NeedFactory" -1.f 1.f ResultSets

    let NeedProduction = fuzzy.CreateVariable "NeedProduction" -1.f 1.f ResultSets
    let BuildProduction = fuzzy.CreateVariable "BuildProduction" -1.f 1.f ResultSets

    let RuleString = """
IF Research is ResearchLow THEN NeedLaboratory is High
IF Research is ResearchNormal THEN NeedLaboratory is Normal
IF Research is ResearchHigh THEN NeedLaboratory is VeryLow
IF Labor is LaborLow THEN NeedFactory is VeryHigh
IF Labor is LaborNormal THEN NeedFactory is High
IF Labor is LaborHigh THEN NeedFactory is VeryLow
IF NeedProduction is High AND ProductionLaborCost is CostLow AND ProductionGoldCost is CostLow THEN BuildProduction is VeryHigh
IF NeedProduction is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostLow THEN BuildProduction is High
IF NeedProduction is High AND ProductionLaborCost is CostLow AND ProductionGoldCost is CostNormal THEN BuildProduction is High
IF NeedProduction is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostNormal THEN BuildProduction is Normal
IF NeedProduction is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostHigh THEN BuildProduction is Low
IF NeedProduction is High AND ProductionLaborCost is CostHigh AND ProductionGoldCost is CostNormal THEN BuildProduction is Low
IF NeedProduction is High AND ProductionLaborCost is CostHigh AND ProductionGoldCost is CostHigh THEN BuildProduction is VeryLow
"""

    type Rules(player : CivModel.Player) =
        let rules = fuzzy.CreateRule <| RuleString.Split([| "\n"; "\r"; "\r\n" |], StringSplitOptions.RemoveEmptyEntries)

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
