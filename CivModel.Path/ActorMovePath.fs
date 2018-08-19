namespace CivModel.Path

open System
open System.Collections.Generic

open CivModel

type ActorMovePath(actor: Actor, endPoint: Terrain.Point, finalAction: IActorAction) =
    do
        if actor = null then
            raise (ArgumentNullException ("actor"))
        if actor.MoveAct = null then
            raise (ArgumentException ("unmovable actor cannot have move path", "actor"))
        if not actor.PlacedPoint.HasValue then
            raise (ArgumentException ("actor is not placed", "actor"))
        if endPoint.Terrain = null then
            raise (ArgumentException ("endPoint is invalid", "endPoint"))
        if actor.PlacedPoint.Value.Terrain <> endPoint.Terrain then
            raise (ArgumentException ("endPoint is not in the same Terrain with actor", "endPoint"))
        if actor.PlacedPoint.Value = endPoint then
            raise (ArgumentException ("endPoint is equal to startpoint", "endPoint"))
        if finalAction = null then
            raise (ArgumentNullException ("finalAction"))
        if finalAction.Owner <> actor then
            raise (ArgumentException ("finalAction is not a action of actor", "finalAction"))

    let getDistance (a: Terrain.Point) (b: Terrain.Point) =
        let action = if b = endPoint then finalAction else actor.MoveAct
        let ap = action.GetRequiredAP(a, Nullable b)
        if ap = ActionPoint.NonAvailable then infinity
        elif ap.Value > actor.MaxAP then infinity
        else ap.Value

    let _path = FreePath(actor.PlacedPoint.Value, endPoint, getDistance)

    let getFirstWalk () =
        match _path.Path with
        | Some (a :: b :: xs) ->
            if actor.MoveAct <> null && (Nullable a) = actor.PlacedPoint then
                Some (a, b, xs)
            else None
        | _ -> None

    let getValidFirstWalk () =
        match getFirstWalk () with
        | Some (a, b, xs) ->
            if getDistance a b <> infinity then
                Choice1Of3 (a, b, xs)
            else Choice2Of3 ()
        | None -> Choice3Of3 ()

    let recalcFirstWalk () =
        match getFirstWalk () with
        | Some _ -> _path.RecalcFirstWalk ()
        | None -> raise (InvalidOperationException ("the path is invalid"))

    let rec actFirstWalk () =
        match getValidFirstWalk () with
        | Choice1Of3 (a, b, xs) ->
            let action = if List.isEmpty xs then finalAction else actor.MoveAct
            if action.IsActable (Nullable b) then
                action.Act (Nullable b)
                _path.PopFirstWalk () |> ignore
                true
            else false
        | Choice2Of3 () ->
            recalcFirstWalk ()
            actFirstWalk ()
        | Choice3Of3 () -> false

    interface IMovePath with
        member this.Actor =  actor

        member this.StartPoint =
            match getFirstWalk () with
            | Some (a, b, xs) -> a
            | None -> raise (InvalidOperationException ("the path is invalid"))

        member this.EndPoint =  endPoint
        member this.FinalAction = finalAction

        member this.IsInvalid =
            match getFirstWalk () with
            | Some _ -> false | None -> true
        member this.IsFirstMoveInvalid =
            match getValidFirstWalk () with
            | Choice1Of3 _ -> false | Choice2Of3 _ -> true
            | Choice3Of3 _ -> raise (InvalidOperationException ("the path is invalid"))

        member this.Path =
            match getFirstWalk () with
            | Some (a, b, xs) -> _path.Path.Value :> IEnumerable<Terrain.Point>
            | None -> raise (InvalidOperationException ("the path is invalid"))

        member this.RecalculateFirstWalk () = recalcFirstWalk ()
        member this.ActFirstWalk () = actFirstWalk ()
