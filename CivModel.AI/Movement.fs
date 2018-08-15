namespace CivModel.AI

open System
open CivModel
open CivModel.Path

module Movement =
    let cityDeployTiles (context: AIContext) =
        let placable (pt: Terrain.Point) =
            let c1 = context.Player.IsAlliedWithOrNull pt.TileOwner
            let c2 = pt.TileBuilding = null && pt.Unit = null
            c1 && c2
        context.Terrain.AllTiles
        |> Seq.filter placable
        |> Seq.map (fun pt -> context.Prefer.CityPref.[pt.Index], pt.Index)
//        |> Seq.filter (fun (p, _) -> p > 0.0)
        |> Array.ofSeq |> Array.sortDescending

    let pathAction (unit: Unit) (path: IMovePath) =
        unit.MovePath <- path
        let rec pathAction' () =
            if unit.MovePath <> null && unit.MovePath.ActFirstWalk () then
                pathAction' ()
        pathAction' ()

    let movePioneer (context: AIContext) (unit: Unit) =
        cityDeployTiles context |> Array.tryPick (fun (_, ptidx) ->
            let pt = context.Terrain.GetPoint ptidx
            if unit.PlacedPoint = Nullable pt then
                None
            else
                let path =
                    ActorMovePath(unit, pt, unit.MoveAct)
                    :> IMovePath
                if not path.IsInvalid then
                    Some (fun () -> pathAction unit path)
                else
                    None
        )

    let moveAction (context: AIContext) (unit: Unit) =
        if unit.MovePath <> null || unit.RemainAP = 0.0 then
            None
        elif unit.GetType().Name.IndexOf "Pioneer" >= 0 then
            movePioneer context unit
        else
            None

    let getAction (context: AIContext) =
        context.Player.Units |> Seq.tryPick (moveAction context)
