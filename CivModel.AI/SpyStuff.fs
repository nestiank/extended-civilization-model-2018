namespace CivModel.AI

open System
open CivModel
open CivModel.Path

module SpyStuff =
    let productionIsSpy (x: Production) =
        x.Factory.ResultType.Name.IndexOf "Spy" >= 0

    let dijkGetAdjacents (set1: Terrain.Point list) (terrain: Terrain) (index: int) =
        if index = 0 then
            set1 |> List.map (fun x -> x.Index + 1)
        else
            let pt = terrain.GetPoint (index - 1)
            pt.Adjacents ()
            |> Array.fold (fun xs p -> if p.HasValue then (p.Value.Index + 1) :: xs else xs) []

    let rec dijkGetDistance (set1: Terrain.Point list) (terrain: Terrain) (idx1: int) (idx2: int) =
        if idx1 = 0 && idx2 = 0 then 0.0
        elif idx1 = 0 then
            let b = terrain.GetPoint (idx2 - 1)
            if set1 |> List.contains b then 0.0 else infinity
        elif idx2 = 0 then
            dijkGetDistance set1 terrain idx2 idx1
        else
            let a = terrain.GetPoint (idx1 - 1)
            let b = terrain.GetPoint (idx2 - 1)
            if b.Type = TerrainType.Mount then infinity
            else float (Terrain.Point.Distance (a, b))

    let findShortest (terrain: Terrain) (set1: Terrain.Point list) (set2: Terrain.Point list) =
        let len = terrain.Width * terrain.Height
        let dist = Array.create (len + 1) infinity
        let prev = Array.create (len + 1) Option<int>.None
        let queue = Dijkstra.createQueue 0
        dist.[0] <- 0.0
        Dijkstra.dijkstra (dijkGetAdjacents set1 terrain) (dijkGetDistance set1 terrain) dist prev queue

        let rec findPath' = function
            | Some idx -> idx :: findPath' prev.[idx]
            | None -> []
        let tryMinBy fn lst =
            if List.isEmpty lst then None
            else Some (List.minBy fn lst)

        set2 |> List.map (fun v ->
            let path =
                findPath' (Some (v.Index + 1))
                |> List.rev |> List.tail
            path, List.length path)
        |> List.filter (fun x -> snd x > 1)
        |> tryMinBy snd
        |> Option.map (fun x -> fst x |> List.map (fun i -> terrain.GetPoint (i - 1)))

    let targetPlayer (context: AIContext) =
        if context.Player.Team = 0 then context.Game.Players.[1]
        else context.Game.Players.[0]

    let getCityAndPath (context: AIContext) (origin: Terrain.Point option) =
        let set1 =
            match origin with
            | Some pt -> [ pt ]
            | None ->
                context.Player.Cities |> List.ofSeq
                |> List.choose (fun x -> Option.ofNullable x.PlacedPoint)
                |> List.filter (fun x -> x.Unit = null)

        let set2 =
            (targetPlayer context).Territory |> List.ofSeq
            |> List.filter (fun x -> x.Unit = null && x.TileBuilding = null)

        let rec last = function x :: [] -> x | x :: xs -> last xs | [] -> failwith "list is empty"

        findShortest context.Terrain set1 set2
        |> Option.map (fun path -> (List.head path), (last path))
