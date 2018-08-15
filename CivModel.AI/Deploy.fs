namespace CivModel.AI

open System
open System.Linq
open CivModel

module Deploy =
    let doDeploy (context: AIContext) (x: Production) (pt: Terrain.Point) =
        context.Player.Deployment.Remove x |> ignore
        x.Place pt |> ignore

    let normalDeployTiles (context: AIContext) (x: Production) =
        context.Terrain.AllTiles
        |> Seq.filter x.IsPlacable
        |> Seq.truncate 1 |> Array.ofSeq
        |> Array.map (fun pt -> 1.0, pt.Index)

    let getTiles (context: AIContext) (x: Production) =
        normalDeployTiles context x

    let tryDeploy (context: AIContext) (x: Production) =
        let point =
            getTiles context x |> Array.tryHead
            |> Option.map (snd >> context.Terrain.GetPoint)
        match point with
        | Some pt ->
            Some (fun () -> doDeploy context x pt)
        | None -> None

    let getAction (context: AIContext) =
        context.Player.Deployment |> Seq.tryPick (tryDeploy context)
