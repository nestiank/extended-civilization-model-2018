namespace CivModel.Path

open FSharpx.Collections

module Dijkstra =
    let createQueue source = Heap.ofSeq false [(0.0, source)]

    let rec dijkstra getAdjacents getDist (dist: float array) (prev: int option array) queue =
        match Heap.tryUncons queue with
        | Some ((d, v), tail) ->
            let relax q pt =
                let nd = d + getDist v pt
                if nd < dist.[pt] then
                    dist.[pt] <- nd
                    prev.[pt] <- Some v
                    q |> Heap.insert (nd, pt)
                else q
            let adj = getAdjacents v
            let nq = adj |> List.fold relax tail
            dijkstra getAdjacents getDist dist prev nq
        | None -> ()
