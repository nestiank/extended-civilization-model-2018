namespace CivModel.AI

open System

open CivModel
open CivModel.Path

module Deploy =
    let tryMin seq =
        if Seq.isEmpty seq then None
        else Some (Seq.min seq)

    let isPlacable (context: AIContext) (x: Production) (pt: Terrain.Point) =
        let precond =
            if context.QuestDeploy.ContainsKey x then
                context.QuestDeploy.[x] x pt
            else true
        precond && x.IsPlacable pt

    let doDeploy (context: AIContext) (x: Production) (pt: Terrain.Point) =
        context.Player.Deployment.Remove x |> ignore
        x.Place pt |> ignore

    let getDeployPoint (context: AIContext) (x: Production) =
        context.Terrain.AllTiles
        |> Seq.filter (isPlacable context x)
        |> Seq.tryHead

    let spyDeploy (context: AIContext) (x: Production) =
        match SpyStuff.getCityAndPath context None with
        | Some (startp, endp) ->
            if x.IsPlacable startp then
                Some (fun () ->
                    doDeploy context x startp

                    let unit = startp.Unit
                    unit.MovePath <- ActorMovePath(unit, endp, unit.MoveAct) :> IMovePath
                )
            else None
        | None -> None

    let tryDeploy (context: AIContext) (x: Production) =
        if SpyStuff.productionIsSpy x then
            spyDeploy context x
        else
            match getDeployPoint context x with
            | Some pt -> Some (fun () -> doDeploy context x pt)
            | None -> None

    let getAction (context: AIContext) =
        context.Player.Deployment
        |> Seq.filter (context.QuestProduction.Contains >> not)
        |> Seq.tryPick (tryDeploy context)
