namespace CivModel.Path

open System
open FSharpx.Collections

open CivModel
open System.Collections.Generic

type public ActorMovePath(actor: Actor, endPoint: Terrain.Point, finalAction: IActorAction) =
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

    let indexToPt (terrain: Terrain) (index: int) =
        terrain.GetPoint(index % terrain.Width, index / terrain.Width)
    let ptToIndex (pos: Terrain.Point) =
        pos.Position.Y * pos.Terrain.Width + pos.Position.X

    let createGraph (terrain: Terrain) (startp: Terrain.Point) =
        let len = terrain.Width * terrain.Height
        let source = ptToIndex startp

        let dist = Array.create len infinity
        let prev = Array.create len Option<int>.None

        dist.[source] <- 0.0;
        dist, prev, source

    let getDistance (a: Terrain.Point) (b: Terrain.Point) =
        let action = if b = endPoint then finalAction else actor.MoveAct
        let ap = action.GetRequiredAP(a, Nullable b)
        if ap = ActionPoint.NonAvailable then infinity
        elif ap.Value > actor.MaxAP then infinity
        else ap.Value

    let rec dijkstra (dist : float array) (prev : int option array) (startp: Terrain.Point) (queue: (float * int) Heap) =
        let terrain = startp.Terrain
        match Heap.tryUncons queue with
        | Some ((d, v), tail) ->
            let vpt = indexToPt terrain v
            let relax (q: (float * int) Heap) (pt: Terrain.Point) =
                let idx = ptToIndex pt
                let nd = d + getDistance vpt pt
                if nd < dist.[idx] then
                    dist.[idx] <- nd
                    prev.[idx] <- Some v
                    q |> Heap.insert (nd, idx)
                else q
            let adj =
                (indexToPt terrain v).Adjacents ()
                |> Array.fold (fun xs p -> if p.HasValue then p.Value :: xs else xs) []
            let nq = adj |> List.fold relax tail
            dijkstra dist prev startp nq
        | None -> ()

    let findPath (startp: Terrain.Point) (endp: Terrain.Point) =
        if startp = endp then []
        else
            let (dist, prev, source) = createGraph startp.Terrain startp
            dijkstra dist prev startp (Heap.ofSeq false [(0.0, source)])

            let rec path = function
                | Some idx -> idx :: path prev.[idx]
                | None -> []
            path (Some (ptToIndex endp))
            |> List.rev
            |> List.map (indexToPt startp.Terrain)

    let mutable (_path : Terrain.Point list) = findPath actor.PlacedPoint.Value endPoint

    let getFirstWalk () =
        match _path with
        | (a :: b :: xs) ->
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
        | Some (a, b, xs) ->
            _path <- (findPath a b) @ xs
        | None -> raise (InvalidOperationException ("the path is invalid"))

    let rec actFirstWalk () =
        match getValidFirstWalk () with
        | Choice1Of3 (a, b, xs) ->
            let action = if List.isEmpty xs then finalAction else actor.MoveAct
            if action.IsActable (Nullable b) then
                action.Act (Nullable b)
                _path <- b :: xs
                true
            else false
        | Choice2Of3 () ->
            recalcFirstWalk ()
            actFirstWalk ()
        | Choice3Of3 () -> raise (InvalidOperationException ("the path is invalid"))

    interface IMovePath with
        member this.Actor =  actor
        member this.StartPoint = List.head _path
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
            | Some _ -> _path :> IEnumerable<Terrain.Point>
            | None -> raise (InvalidOperationException ("the path is invalid"))

        member this.RecalculateFirstWalk () = recalcFirstWalk ()
        member this.ActFirstWalk () = actFirstWalk ()
