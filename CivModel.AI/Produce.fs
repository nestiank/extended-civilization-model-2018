namespace CivModel.AI

open System
open CivModel

module Produce =
    let countTotal (context: AIContext) (resultType: Type) =
        let cntfn (t: Type) =
            if resultType.IsAssignableFrom t then 1
            else 0
        let exist = context.Player.Actors |> Seq.sumBy (fun x -> cntfn (x.GetType ()))
        let deploy = context.Player.Deployment |> Seq.sumBy (fun x -> cntfn x.Factory.ResultType)
        let product = context.Player.Production |> Seq.sumBy (fun x -> cntfn x.Factory.ResultType)
        exist, deploy, product

    let cityToProduce (context: AIContext) =
        match context.AvailableCity, context.AvailablePioneer with
        | Some city, Some pioneer ->
            let (cityExist, cityDeploy, cityProduct) = countTotal context city.ResultType
            let (pioneerExist, pioneerDeploy, pioneerProduct) = countTotal context pioneer.ResultType
            if cityDeploy + cityProduct > pioneerExist + pioneerDeploy + pioneerProduct then
                None, Some pioneer
            elif cityDeploy + cityProduct < 2 then
                Some city, Some pioneer
            else
                None, None
        | _ -> None, None

    let produceCityPioneer (context: AIContext) =
        let player = context.Player
        match cityToProduce context with
        | Some city, Some pioneer ->
            Some (fun () ->
                player.Production.AddLast (pioneer.Create player) |> ignore
                player.Production.AddLast (city.Create player) |> ignore)
        | None, Some pioneer ->
            Some (fun () ->
                player.Production.AddLast (pioneer.Create player) |> ignore)
        | _ -> None

    let produceFactory (context: AIContext) =
        let player = context.Player
        match context.AvailableFactory with
        | Some factory ->
            Some (fun () ->
                player.Production.AddLast (factory.Create player) |> ignore)
        | None -> None

    let produceBattler (context: AIContext) =
        let player = context.Player
        match context.AvailableBattler with
        | Some x ->
            let count =
                Seq.concat [ player.Production; player.Deployment ]
                |> Seq.filter (fun p -> p.Factory = x)
                |> Seq.length
            if count < 5 then
                Some (fun () -> player.Production.AddLast (x.Create player) |> ignore)
            else None
        | None -> None

    let getAction (context: AIContext) =
        let player = context.Player
        let remainLabor = player.Labor - player.EstimatedUsedLabor
        if player.Labor < 30.0 && remainLabor / player.Labor > 0.1 then
            produceFactory context
        elif player.Labor < 200.0 && remainLabor / player.Labor > 0.8 then
            produceFactory context
        elif remainLabor > 500.0 && remainLabor / player.Labor > 0.8 then
            produceCityPioneer context
        elif remainLabor / player.Labor > 0.6 then
            produceBattler context
        else
            None
