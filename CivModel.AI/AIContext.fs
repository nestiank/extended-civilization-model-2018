namespace CivModel.AI

open System
open CivModel

type AIContext(player: Player) =
    let isFactoryOfType (expected: Type) (factory: IProductionFactory) =
        expected.IsAssignableFrom factory.ResultType

    let productIsName (s: string) (x: IProductionFactory) =
        let t = x.ResultType
        t.FullName.IndexOf "Common" = -1 && t.Name.IndexOf s >= 0

    member this.Player = player
    member this.Game = player.Game
    member this.Terrain = player.Game.Terrain

    member val Prefer = Preference player

    member val QuestComplete : System.Collections.Generic.List<Quest> = null
        with get, set

    member val QuestProduction = System.Collections.Generic.HashSet<Production>()
    member val QuestDeploy = System.Collections.Generic.Dictionary<Production, Production -> Terrain.Point -> bool>()

    member this.AvailableCity =
        player.AvailableProduction
        |> Seq.filter (isFactoryOfType typeof<CityBase>)
        |> Seq.tryHead

    member this.AvailablePioneer =
        player.AvailableProduction
        |> Seq.filter (productIsName "Pioneer")
        |> Seq.tryHead

    member this.AvailableFactory =
        player.AvailableProduction
        |> Seq.filter (productIsName "FIRFactory")
        |> Seq.tryHead

    member this.AvailableBattler =
        player.AvailableProduction
        |> Seq.tryFind (fun x ->
            x.ResultType.Name.IndexOf "Jedi" >= 0 || x.ResultType.Name.IndexOf "Decentr" >= 0)


    member this.Update () =
        player.EstimateResourceInputs ()
        this.Prefer.Update ()
