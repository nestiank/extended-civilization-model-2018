namespace CivModel.AI

open System
open System.Linq
open CivModel
open CivModel.Common

open AIFuzzyModule

type GlobalRules(player : CivModel.Player) =
    let random = Random()

    let enemy =
        let idx = player.Game.Players |> Seq.findIndex ((=) player)
        if idx = 0 then
            player.Game.Players.[1]
        else
            player.Game.Players.[0]

    let playerIndex = player.Game.Players |> Seq.findIndex (fun x -> x = player)

    let RuleString =
        let addition' = [ RuleStringMoveTaskCommon; RuleStringBuildTaskCommon; RuleStringDeployTaskCommon; RuleStringUseSkillCommon ]
        let addition =
            match playerIndex with
            | Utils.HwanPlayer | Utils.FinnoPlayer ->
                RuleStringGlobalSuomiHwan :: RuleStringMoveTaskOther :: addition'
            | _ ->
                RuleStringGlobalOther :: addition'
        String.concat "\n" (RuleStringGlobalCommon :: addition)

    let rules =
        RuleString.Split([| "\r"; "\n" |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun s -> s.Trim())
        |> fuzzySystem.CreateRule

    let DoFightingUnit x =
        if player.Production |> Seq.length > 20 then
            false
        else
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
        if player.Production |> Seq.length > 20 then
            false
        else
            let ar =
                player.AvailableProduction
                |> Enumerable.OfType<IActorProductionFactory>
                |> Seq.filter (fun p -> typeof<CityBase>.IsAssignableFrom p.ResultType)
                |> Seq.toArray
            if not (Array.isEmpty ar) then
                let prod = ar.[random.Next (Array.length ar - 1)].Create(player)
                player.Production.AddLast prod |> ignore
                true
            else
                false

    let DoPioneer x =
        if player.Production |> Seq.length > 20 then
            false
        else
            let ar =
                player.AvailableProduction
                |> Enumerable.OfType<IActorProductionFactory>
                |> Seq.filter (fun p -> typeof<Pioneer>.IsAssignableFrom p.ResultType)
                |> Seq.toArray
            if not (Array.isEmpty ar) then
                let prod = ar.[random.Next (Array.length ar - 1)].Create(player)
                player.Production.AddLast prod |> ignore
                true
            else
                false

    let DoMilitaryBuilding x =
        if player.Production |> Seq.length > 20 then
            false
        else
            let ar =
                player.AvailableProduction
                |> Enumerable.OfType<IActorProductionFactory>
                |> Seq.filter (fun p ->
                    typeof<Hwan.HwanEmpireVigilant>.IsAssignableFrom p.ResultType
                     || typeof<Hwan.HwanEmpireFIRFortress>.IsAssignableFrom p.ResultType
                     || typeof<Finno.AncientFinnoVigilant>.IsAssignableFrom p.ResultType
                     || typeof<Finno.AncientFinnoFIRFortress>.IsAssignableFrom p.ResultType
                     || typeof<Zap.FIRFortress>.IsAssignableFrom p.ResultType)
                |> Seq.toArray
            if not (Array.isEmpty ar) then
                let prod = ar.[random.Next (Array.length ar - 1)].Create(player)
                player.Production.AddLast prod |> ignore
                true
            else
            false

    let DoEconInvesttoDouble x =
        player.EconomicInvestmentRatio <- 2.0
        false

    let DoEconInvesttoFull x =
        player.EconomicInvestmentRatio <- 1.0
        false

    let fuzzyActionList = [
        NeedFightingUnit, DoFightingUnit;
        NeedCity, DoCity;
        NeedPioneer, DoPioneer;
        NeedMilitaryBuilding, DoMilitaryBuilding;
        SetEconInvesttoDouble, DoEconInvesttoDouble;
        SetEconInvesttoFull, DoEconInvesttoFull;
    ]

    let mutable prevTech = -infinity
    let getTechLost() =
        let diff = prevTech - player.Research
        let ret = max diff 0.0
        prevTech <- player.Research
        ret

    let setFuzzyInput() =
        player.EstimateResourceInputs()
        let potential = Seq.append player.Production player.Deployment

        let efun = (Utils.fightUnitsOfPlayer enemy |> Seq.length) 
        rules |> EnemyFightingUnitNum.SetValue (Utils.fightUnitsOfPlayer enemy |> Seq.length |> float32)
        
        let amud' (u : Unit) =
            if u.PlacedPoint.HasValue then
                match Utils.searchNear (fun p -> p.Unit <> null && p.Unit.Owner = enemy) u.PlacedPoint.Value with
                | Some p -> Some (Terrain.Point.Distance (u.PlacedPoint.Value, p.Unit.PlacedPoint.Value))
                | None -> None
            else
                None
        let amudSeq = player.Units |> Seq.choose amud'
        let amud =
            if Seq.isEmpty amudSeq then float (player.Game.Terrain.Width + player.Game.Terrain.Height)
            else amudSeq |> Seq.averageBy float
        rules |> AllMyUnitEnemDist.SetValue (float32 amud)

        rules |> DmgUnitNum.SetValue (float32 (player.Units |> Seq.filter (fun u -> u.RemainHP < u.MaxHP) |> Seq.length))
        rules |> TechLost.SetValue (float32 (getTechLost ()))

        rules |> DeltaHappyGoal.SetValue (100.f - float32 player.Happiness)
        rules |> RemainingGold.SetValue (float32 player.GoldNetIncome)
        rules |> RemainingLabor.SetValue (float32 (player.Labor - player.EstimatedUsedLabor))
        rules |> Gold.SetValue (float32 player.Gold)
        rules |> Tech.SetValue (float32 player.Research)
        rules |> Labor.SetValue (float32 player.Labor)
        rules |> Happy.SetValue (float32 player.Happiness)
        rules |> TechInvest.SetValue (float32 player.ResearchInvestmentRatio)
        rules |> Logistics.SetValue (float32 player.RepairInvestmentRatio)

        match playerIndex with
        | Utils.HwanPlayer | Utils.FinnoPlayer ->
            let dfun =
                float ((Utils.fightUnitsOfPlayer player |> Seq.length) + (Utils.fightUnitsOfProducts potential |> Seq.length))
                - (1.5 * float (Utils.fightUnitsOfPlayer enemy |> Seq.length) + (float <| Seq.length enemy.Cities))
            rules |> DeltaFightingUnitNum.SetValue (float32 dfun)

            let potcity =
                10 * max
                    (Utils.productsOfType<CityBase> potential |> Seq.length)
                    (Utils.productsOfType<Common.Pioneer> potential |> Seq.length)
            let dcn =
                (player.Cities |> Seq.length |> float) + (player.Units |> Enumerable.OfType<Common.Pioneer> |> Seq.length |> float)
                + float potcity - 1.5 * (enemy.Cities |> Seq.length |> float)
            rules |> DeltaCityNum.SetValue (float32 dcn)
        | _ ->
            let mfun = float ((Utils.fightUnitsOfPlayer player |> Seq.length) + (Utils.fightUnitsOfProducts potential |> Seq.length))
            rules |> MyFightingUnitNum.SetValue (float32 mfun)

            let potcity =
                10 * max
                    (Utils.productsOfType<CityBase> potential |> Seq.length)
                    (Utils.productsOfType<Common.Pioneer> potential |> Seq.length)
            let mcn = (player.Cities |> Seq.length) + potcity
            rules |> MyCityNum.SetValue (float32 mcn)

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

    member val FuzzyRules = rules
    member this.DoFuzzyAction() = doFuzzyAction 0 fuzzyActionList
