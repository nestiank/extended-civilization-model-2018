namespace CivModel.Path

open System
open FSharpx.Collections

open CivModel

type FreePath(startPoint: Terrain.Point, endPoint: Terrain.Point, getDistance: (Terrain.Point -> Terrain.Point -> float)) =
    do
        if startPoint.Terrain = null then
            raise (ArgumentException ("startPoint is invalid", "startPoint"))
        if startPoint.Terrain <> endPoint.Terrain then
            raise (ArgumentException ("endPoint is invalid", "endPoint"))
        if startPoint = endPoint then
            raise (ArgumentException ("endPoint is equal to startpoint", "endPoint"))

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
        if startp = endp then None
        else
            let (dist, prev, source) = createGraph startp.Terrain startp
            dijkstra dist prev startp (Heap.ofSeq false [(0.0, source)])

            let rec findPath' = function
                | Some idx -> idx :: findPath' prev.[idx]
                | None -> []
            let path =
                findPath' (Some (ptToIndex endp))
                |> List.rev
                |> List.map (indexToPt startp.Terrain)
            match path with
            | _ :: _ :: _ -> Some path
            | _ -> None

    let mutable _path = findPath startPoint endPoint

    member this.Path = _path
    member this.Head = _path |> Option.map List.head

    member this.RecalcFirstWalk () =
        match _path with
        | Some (a :: b :: xs) ->
            match findPath a b with
            | Some y -> _path <- Some (y @ xs)
            | None -> _path <- None
        | _ -> ()

    member this.PopFirstWalk () =
        match _path with
        | Some [ a; b ] ->
            _path <- None
            Some (a, b)
        | Some (a :: b :: xs) ->
            _path <- Some (b :: xs)
            Some (a, b)
        | _ -> None
