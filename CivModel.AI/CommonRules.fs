namespace CivModel.AI

open System
open System.Linq
open CivModel
open CivModel.Common

open AIFuzzyModule

type CommonRules(player : CivModel.Player) =
    let random = Random()

    let rules = fuzzySystem.CreateRule <| RuleString.Split([| "\r"; "\n" |], StringSplitOptions.RemoveEmptyEntries)

    let enemy =
        let idx = player.Game.Players |> Seq.findIndex ((=) player)
        if idx = 0 then
            player.Game.Players.[1]
        else
            player.Game.Players.[0]

    let DoFightingUnit x =
        let ar =
            player.AvailableProduction
            |> Enumerable.OfType<IActorProductionFactory>
            |> Seq.filter (fun p -> typeof<Unit>.IsAssignableFrom p.ResultType && p.ActorConstants.MaxHP > 0.0)
            |> Seq.toArray
        if not (Array.isEmpty ar) then
            let prod = ar.[random.Next (Array.length ar - 1)].Create(player)
            player.Production.AddLast prod |> ignore
            true
        else
            false

    let DoCity x =
        let ar =
            player.AvailableProduction
            |> Enumerable.OfType<IActorProductionFactory>
            |> Seq.filter (fun p -> typeof<CityBase>.IsAssignableFrom p.ResultType && p.ActorConstants.MaxHP > 0.0)
            |> Seq.toArray
        if not (Array.isEmpty ar) then
            let prod = ar.[random.Next (Array.length ar - 1)].Create(player)
            player.Production.AddLast prod |> ignore
            true
        else
            false

    let DoPioneer x =
        let ar =
            player.AvailableProduction
            |> Enumerable.OfType<IActorProductionFactory>
            |> Seq.filter (fun p -> typeof<Pioneer>.IsAssignableFrom p.ResultType && p.ActorConstants.MaxHP > 0.0)
            |> Seq.toArray
        if not (Array.isEmpty ar) then
            let prod = ar.[random.Next (Array.length ar - 1)].Create(player)
            player.Production.AddLast prod |> ignore
            true
        else
            false

    let fuzzyActionList = [
        NeedFightingUnit, DoFightingUnit;
        NeedCity, DoCity;
        NeedPioneer, DoPioneer;
    ]

    let setFuzzyInput() =
        player.EstimateResourceInputs()

        let dfun = float (Utils.fightUnits player |> Seq.length) - (1.5 * float (Utils.fightUnits enemy |> Seq.length) + (float <| Seq.length enemy.Cities))
        rules |> DeltaFightingUnitNum.SetValue (float32 dfun)

        let dcn =
            (player.Cities |> Seq.length |> float) + (player.Units |> Enumerable.OfType<Common.Pioneer> |> Seq.length |> float)
            - 1.5 * (enemy.Cities |> Seq.length |> float)
        rules |> DeltaCityNum.SetValue (float32 dcn)

        rules |> EnemyFightingUnitNum.SetValue (Utils.fightUnits enemy |> Seq.length |> float32)
        
        let amud' (u : Unit) =
            if u.PlacedPoint.HasValue then
                match Utils.searchNear (fun p -> p.Unit <> null && p.Unit.Owner = enemy) u.PlacedPoint.Value with
                | Some p -> Some (Terrain.Point.Distance (u.PlacedPoint.Value, p.Unit.PlacedPoint.Value))
                | None -> None
            else
                None
        let amud = player.Units |> Seq.choose amud' |> Seq.averageBy float
        rules |> AllMyUnitEnemDist.SetValue (float32 amud)

        rules |> DeltaHappyGoal.SetValue (100.f - float32 player.Happiness)
        rules |> RemainingGold.SetValue (float32 player.GoldNetIncome)
        rules |> RemainingLabor.SetValue (float32 (player.Labor - player.EstimatedUsedLabor))

    let rec doFuzzyAction count (space : (FuzzyVariable * (float32 -> bool)) list) =
        if count >= 500 then
            System.Diagnostics.Debug.WriteLine "too many fuzzy actions are detected"
            ()
        elif not space.IsEmpty then
            setFuzzyInput()
            let (k, v, x) =
                space
                |> List.map (fun (k, v) -> (k, v, rules |> k.GetValue))
                |> List.maxBy (fun (_, _, x) -> x)
            if x > 0.f then
                let d = v x
                let next =
                    if d then space
                    else space |> List.filter (fun (k', _) -> k <> k')
                doFuzzyAction (count + 1) next
            else
                ()
        else
            ()

    member this.DoFuzzyAction() = doFuzzyAction 0 fuzzyActionList
