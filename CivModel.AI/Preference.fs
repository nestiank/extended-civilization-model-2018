namespace CivModel.AI

open System
open CivModel

type Preference(player: Player) =
    let preferBySelf distance = if distance < 3.0 then -infinity else 1.0 / (1.0 + distance)
    let preferByAlly distance = if distance < 3.0 then -infinity else 1.0 / (1.0 + distance)
    let preferByEnemy distance=  if distance < 3.0 then -infinity else -1.0 / (1.0 + distance)

    member val CityPref =
        let game = player.Game
        let terrain = game.Terrain
        let len = terrain.Width * terrain.Height
        let arr = Array.create len 0.0
        game.Players |> Seq.iter (fun p ->
            let prefer =
                if p = player then preferBySelf
                elif player.IsAlliedWith p then preferByAlly
                else preferByEnemy
            p.Cities |> Seq.iter (fun city ->
                for index = 0 to len - 1 do
                    let dist = Terrain.Point.Distance(city.PlacedPoint.Value, terrain.GetPoint index)
                    arr.[index] <- prefer (float dist)
            )
        )
        arr
