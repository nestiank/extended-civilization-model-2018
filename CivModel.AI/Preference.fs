namespace CivModel.AI

open System
open CivModel

type Preference(player: Player) =
    let cityPrefer = [|
        fun x -> if x <= 1.0 then -infinity else 1.0 / (1.0 + x);
        fun x -> if x <= 1.0 then -infinity else 1.0 / (1.0 + x);
        fun x -> if x <= 1.0 then -infinity else -1.0 / (1.0 + x);
    |]

    let roamPreferCity = [|
        fun x -> if x <= 1.0 then -infinity else -1.0 / (1.0 + x);
        fun x -> if x <= 1.0 then -infinity else -1.0 / (1.0 + x);
        fun x -> if x <= 1.0 then -infinity else -1.0 / (1.0 + x);
    |]
    let roamPreferBuilding = [|
        fun x -> if x <= 1.0 then -infinity else 0.0;
        fun x -> if x <= 1.0 then -infinity else 0.0;
        fun x -> if x <= 1.0 then -infinity else -1.0 / (1.0 + x);
    |]
    let roamPreferUnit = [|
        fun x -> if x <= 1.0 then -infinity else -(cos x) / (1.0 + pown x 2);
        fun x -> if x <= 1.0 then -infinity else -(cos x) / (4.0 + pown x 2);
        fun x -> if x <= 1.0 then -infinity else 1.0 / (1.0 + x);
    |]

    let getPrefer (p: Player) (data: (float -> float) array) =
        if p = player then data.[0]
        elif player.IsAlliedWith p then data.[1]
        else data.[2]

    let calcCityPref () =
        let game = player.Game
        let terrain = game.Terrain
        let len = terrain.Width * terrain.Height
        let arr = Array.create len 0.0
        game.Players |> Seq.iter (fun p ->
            let prefer = getPrefer p cityPrefer
            p.Cities |> Seq.iter (fun city ->
                for index = 0 to len - 1 do
                    let dist = Terrain.Point.Distance(city.PlacedPoint.Value, terrain.GetPoint index)
                    arr.[index] <- prefer (float dist)
            )
        )
        arr

    let calcRoamPref () =
        let game = player.Game
        let terrain = game.Terrain
        let len = terrain.Width * terrain.Height
        let arr = Array.create len 0.0
        game.Players |> Seq.iter (fun p ->
            let preferCity = getPrefer p roamPreferCity
            let preferBuilding = getPrefer p roamPreferBuilding
            let preferUnit = getPrefer p roamPreferUnit
            p.Actors |> Seq.iter (fun actor ->
                let prefer =
                    match actor with
                    | :? CityBase -> preferCity
                    | :? TileBuilding -> preferBuilding
                    | _ -> preferUnit
                for index = 0 to len - 1 do
                    let dist = Terrain.Point.Distance(actor.PlacedPoint.Value, terrain.GetPoint index)
                    arr.[index] <- prefer (float dist)
            )
        )
        arr

    let mutable _cityPref : float [] = null
    let mutable _roamPref : float [] = null

    member this.CityPref = _cityPref
    member this.RoamPref = _roamPref

    member this.Update () =
        _cityPref <- calcCityPref ()
        _roamPref <- calcRoamPref ()
