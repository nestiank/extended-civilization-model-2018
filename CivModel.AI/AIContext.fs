namespace CivModel.AI

open System
open CivModel

type AIContext(player: Player) =
    let isFactoryOfType (expected: Type) (factory: IProductionFactory) =
        expected.IsAssignableFrom factory.ResultType

    do
        player.EstimateResourceInputs ()

    member this.Player = player
    member this.Game = player.Game
    member this.Terrain = player.Game.Terrain

    member val Prefer = Preference player

    member this.AvailableCity =
        player.AvailableProduction
        |> Seq.filter (isFactoryOfType typeof<CityBase>)
        |> Seq.tryHead

    member this.AvailablePioneer =
        player.AvailableProduction
        |> Seq.filter (fun factory -> factory.ResultType.Name.IndexOf "Pioneer" >= 0)
        |> Seq.tryHead
