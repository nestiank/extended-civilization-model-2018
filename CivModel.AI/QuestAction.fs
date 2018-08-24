namespace CivModel.AI

open System
open CivModel
open CivModel.Quests

open CivModel.Finno
open CivModel.Hwan
open CivModel.Zap

module QuestAction =
    let questProduce (context: AIContext) (quest: Quest) (product: IProductionFactory) count deploy =
        if context.QuestComplete.Contains quest then
            if quest.Status <> QuestStatus.Accepted then
                context.QuestComplete.Remove quest |> ignore

        if context.QuestComplete.Contains quest then
            None
        elif quest.Status = QuestStatus.Deployed || quest.Status = QuestStatus.Accepted then
            let already =
                Seq.concat [ context.Player.Production; context.Player.Deployment ]
                |> Seq.filter (fun x -> x.Factory = product)
            already |> Seq.iter (fun x -> context.QuestProduction.Add x |> ignore)

            let require = count - Seq.length already
            let finished =
                require = 0 && already |> Seq.forall (fun x -> x.IsCompleted)
            let isDeployDue =
                quest.Status = QuestStatus.Deployed && (quest.LeftTurn = 0 || quest.LeftTurn = 1)

            if finished then
                context.QuestComplete.Add quest

            if require > 0 || finished || isDeployDue then
                Some (fun () ->
                    for i = 1 to require do
                        let p = product.Create context.Player
                        context.Player.Production.AddLast p |> ignore
                        context.QuestProduction.Add p |> ignore
                    if isDeployDue || finished then
                        quest.Status <- QuestStatus.Accepted
                    if finished then
                        already |> Seq.iteri (fun idx x ->
                            context.QuestProduction.Remove x |> ignore
                            context.QuestDeploy.[x] <- deploy idx
                        )
                )
            else None
        else None

    let getActionAutismReflect (context: AIContext) (quest: Quest) =
        if quest.Status = QuestStatus.Deployed then
            Some (fun () ->
                quest.Status <- QuestStatus.Accepted
            )
        else None

    let getActionCthulhu (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Hwan.SpyProductionFactory.Instance 1
            (fun _ _ _ -> true)

    let getActionEgypt (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Hwan.PreternaturalityProductionFactory.Instance 1
            (fun _ _ pt -> pt.TileOwner = context.Game.GetPlayerEgypt ())

    let getActionWarAliance (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Finno.AutismBeamDroneFactory.Instance 3
            (fun _ _ _ -> true)

    let getActionAtlantis (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Finno.PreternaturalityProductionFactory.Instance 1
            (fun _ _ pt -> pt.TileOwner = context.Game.GetPlayerAtlantis ())

    let getActionGateOfRlyeh (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Finno.PreternaturalityProductionFactory.Instance 1
            (fun _ _ pt -> pt.TileOwner = context.Game.GetPlayerAtlantis ())

    let getActionOrbitDominate (context: AIContext) (quest: Quest) =
        None

    let getActionMoaiForce (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Hwan.HwanEmpireFIRFortressProductionFactory.Instance 1
            (fun _ _ pt -> pt.TileOwner = context.Game.GetPlayerEaster ())

    let getActionIntersteller (context: AIContext) (quest: Quest) =
        questProduce context quest CivModel.Finno.PreternaturalityProductionFactory.Instance 2
            (fun idx _ pt ->
                if idx = 0 then pt.TileOwner = context.Game.GetPlayerFinno ()
                else pt.TileOwner = context.Game.GetPlayerEmu ())

    let getActionGenetic (context: AIContext) (quest: Quest) =
        if quest.Status = QuestStatus.Deployed then
            Some (fun () ->
                quest.Status <- QuestStatus.Accepted
            )
        else None

    let getActionPrenatDefer (context: AIContext) =
        let player = context.Player
        let isPrenat (x: Production) =
            let t = x.Factory.ResultType.GetType ()
            t.Name.IndexOf "Preternaturality" >= 0
        let prenats = player.Production |> Seq.filter isPrenat
        if not (Seq.isEmpty prenats) then
            Some (fun () ->
                prenats |> Seq.iter (player.Production.Remove >> ignore)
                prenats |> Seq.iter (player.Production.AddLast >> ignore)
            )
        else None

    let getAction (context: AIContext) =
        context.Player.Quests |> Seq.tryPick (function
        | :? QuestAutismBeamReflex as q -> getActionAutismReflect context q
        | :? QuestPorjectCthulhu as q -> getActionCthulhu context q
        | :? QuestEgyptKingdom as q -> getActionEgypt context q
        | :? QuestWarAliance as q -> getActionWarAliance context q
        | :? QuestAtlantis as q -> getActionAtlantis context q
        | :? QuestRlyeh as q -> getActionGateOfRlyeh context q
        | :? QuestSubAirspaceDomination as q -> getActionOrbitDominate context q
        | :? QuestSubMoaiForceField as q -> getActionMoaiForce context q
//      | :? QuestSubInterstellarEnergy as q -> getActionIntersteller context q
        | :? QuestSubGeneticEngineering as q -> getActionGenetic context q
        | _ -> None)
        |> Option.orElseWith (fun () -> getActionPrenatDefer context)
