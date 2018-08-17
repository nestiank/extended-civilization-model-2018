namespace CivModel.AI

open System
open CivModel

type Preference(player: Player) =
    let cityPrefer = [|
        fun x -> 1.0 / (1.0 + x);
        fun x -> 1.0 / (1.0 + x);
        fun x -> -1.0 / (1.0 + x);
    |]

    let roamPreferCity = [|
        fun x -> -1.0 / (1.0 + x);
        fun x -> -1.0 / (1.0 + x);
        fun x -> -1.0 / (1.0 + x);
    |]
    let roamPreferBuilding = [|
        fun x -> 0.0;
        fun x -> 0.0;
        fun x -> -1.0 / (1.0 + x);
    |]
    let roamPreferUnit = [|
        fun x -> -(cos x) / (1.0 + pown x 2);
        fun x -> -(cos x) / (4.0 + pown x 2);
        fun x -> 1.0 / (1.0 + x);
    |]

    let game = player.Game
    let terrain = game.Terrain
    let terrainSize = terrain.Width * terrain.Height

    let getPrefer (p: Player) (data: (float -> float) array) =
        if p = player then data.[0]
        elif player.IsAlliedWith p then data.[1]
        else data.[2]

    let calcCityPref (index: int) =
        let pt = terrain.GetPoint index
        if pt.TileBuilding :? CityBase then
            -infinity
        else
            game.Players |> Seq.sumBy (fun p ->
                let prefer = getPrefer p cityPrefer
                p.Cities |> Seq.sumBy (fun city ->
                    let dist = Terrain.Point.Distance(city.PlacedPoint.Value, pt)
                    prefer (float dist)
                )
            )

    let calcRoamPref (index: int) =
        let pt = terrain.GetPoint index
        if pt.Unit <> null || pt.TileBuilding <> null then
            -infinity
        else
            game.Players |> Seq.sumBy (fun p ->
                let preferCity = getPrefer p roamPreferCity
                let preferBuilding = getPrefer p roamPreferBuilding
                let preferUnit = getPrefer p roamPreferUnit
                p.Actors |> Seq.sumBy (fun actor ->
                    let prefer =
                        match actor with
                        | :? CityBase -> preferCity
                        | :? TileBuilding -> preferBuilding
                        | _ -> preferUnit
                    let dist = Terrain.Point.Distance(actor.PlacedPoint.Value, pt)
                    prefer (float dist)
                )
            )

    let mutable _cityPref : float [] = null
    let mutable _roamPref : float [] = null

    member this.CityPref index =
        if _cityPref.[index] = nan then
            _cityPref.[index] <- calcCityPref index
        _cityPref.[index]

    member this.RoamPref index =
        if _roamPref.[index] = nan then
            _roamPref.[index] <- calcRoamPref index
        _roamPref.[index]

    member this.Update () =
        _cityPref <- Array.create terrainSize nan
        _roamPref <- Array.create terrainSize nan
