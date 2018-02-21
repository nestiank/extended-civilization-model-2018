namespace CivModel.AI

open System
open System.Collections.Generic
open CivModel

type public GameSchemeFactory() =
    interface IGameSchemeFactory with
        member val Guid = Guid("1A698AD2-DA40-455C-BD60-1A40C14FB76D") with get
        member val SchemeType = typeof<GameScheme> with get

        member val Dependencies = [ ] :> IEnumerable<Guid> with get
        member val KnownSchemeFactories = [ ] :> IEnumerable<IGameSchemeFactory> with get

        member this.Create() = GameScheme(this) :> IGameScheme

and public GameScheme(factory) =
    interface IGameAIScheme with
        member this.Factory = factory :> IGameSchemeFactory
        member this.CreateAI player = AIController(player) :> IAIController
