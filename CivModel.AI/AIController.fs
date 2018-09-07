namespace CivModel.AI

open System
open System.Threading.Tasks
open System.Diagnostics
open CivModel

type AIController(player: Player) =
    let actions = [
        Produce.getAction;
        QuestAction.getAction;
        Deploy.getAction;
        Battle.getAction;
        Movement.getAction;
    ]

    let questComplete = System.Collections.Generic.List<Quest>()

    let getAction (context: AIContext) =
        context.Update ()
        actions |> List.tryPick ((|>) context)

    let investAdjust (context: AIContext) =
        let player = context.Player
        let clamp a b x =
            if a > x then a
            elif x > b then b
            else x
        let x =
            if player.Happiness < 0.0 then 2.0
            elif player.Happiness < 30.0 then 1.1
            else 1.0
        let N = context.Game.Constants.GoldCoefficient
        let LM = context.Game.Constants.EconomicRequireCoefficient
        let t = clamp 0.0 1.0 (N / LM / 2.0 / x)
        player.TaxRate <- t
        player.EconomicInvestmentRatio <- x

    interface CivModel.IAIController with
        member this.DoAction() =
            async {
                try
                    let context = AIContext player
                    context.QuestComplete <- questComplete

                    let rec action ctx count =
                        if count > 10000 then
                            Debug.WriteLine "AI infinite loop is detected"
                        else
                            investAdjust context
                            let act = getAction ctx
                            match act with
                            | Some fn ->
                                fn ()
                                action ctx (count + 1)
                            | None -> ()
                    action context 0
                with
                | ex ->
                    Debug.WriteLine ("AI uncaught exception: " + (ex.ToString ()))
                    questComplete.Clear ()
            } |> Async.StartAsTask :> Task
        member this.Destroy() = ()
