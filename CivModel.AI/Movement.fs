namespace CivModel.AI

open System
open CivModel
open CivModel.Path

module Movement =
    let pathAction (unit: Unit) (endpoint: Terrain.Point) =
        let path = ActorMovePath(unit, endpoint, unit.MoveAct) :> IMovePath
        let rec pathAction' () =
            if unit.MovePath <> null && unit.MovePath.ActFirstWalk () then
                pathAction' ()
        if not path.IsInvalid then
            Some (fun () ->
                unit.MovePath <- path
                pathAction' ())
        else
            None

    let movePioneer (context: AIContext) (unit: Unit) =
        let placable (pt: Terrain.Point) =
            let c1 = context.Player.IsAlliedWithOrNull pt.TileOwner
            let c2 = pt.TileBuilding = null && pt.Unit = null
            c1 && c2
        context.Terrain.AllTiles
        |> Seq.filter placable
        |> Seq.map (fun pt -> context.Prefer.CityPref pt.Index, pt.Index)
        |> Array.ofSeq |> Array.sortDescending
        |> Array.tryPick (fun (_, ptidx) ->
            let pt = context.Terrain.GetPoint ptidx
            if unit.PlacedPoint = Nullable pt then
                None
            else
                pathAction unit pt
        )

    let moveNormal (context: AIContext) (unit: Unit) =
        if unit.MoveAct <> null then
            unit.PlacedPoint.Value.Adjacents () |> Array.choose Option.ofNullable
            |> Array.map (fun pt -> context.Prefer.RoamPref pt.Index, pt.Index)
            |> Array.sortDescending
            |> Array.tryPick (fun (_, ptidx) ->
                let pt = context.Terrain.GetPoint ptidx
                if unit.MoveAct.IsActable (Nullable pt) then
                    Some (fun () -> unit.MoveAct.Act (Nullable pt))
                else None
            )
        else None

    let moveSpy (context: AIContext) (unit: Unit) =
        let action = unit.SpecialActs.[0]
        if action.IsActable (Nullable ()) then
            let isQuestAccepted =
                context.Player.Quests
                |> Seq.exists (
                    function
                    | :? CivModel.Quests.QuestPorjectCthulhu as x ->
                        x.Status = QuestStatus.Accepted
                    | _ -> false)
            if isQuestAccepted then
                Some (fun () -> action.Act (Nullable ()))
            else
                None
        else
            match SpyStuff.getCityAndPath context (Some (unit.PlacedPoint.Value)) with
            | Some (_, endp) ->
                let path = ActorMovePath(unit, endp, unit.MoveAct) :> IMovePath
                if not path.IsInvalid then
                    Some (fun() -> unit.MovePath <- path)
                else None
            | None -> moveNormal context unit

    let moveAction (context: AIContext) (unit: Unit) =
        let pioneerType =
            context.AvailablePioneer |> Option.map (fun x -> x.ResultType)
        if unit.MovePath <> null || unit.RemainAP = 0.0 then
            None
        elif Some (unit.GetType ()) = pioneerType then
            movePioneer context unit
        elif (unit.GetType ()).Name.IndexOf "Spy" >= 0 then
            moveSpy context unit
        else
            moveNormal context unit

    let getAction (context: AIContext) =
        context.Player.Units |> Seq.tryPick (moveAction context)
