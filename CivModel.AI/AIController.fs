namespace CivModel.AI

open System
open System.Linq
open System.Threading.Tasks
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

    interface CivModel.IAIController with
        member this.DoAction() =
            async {
                let context = AIContext player
                context.QuestComplete <- questComplete

                let rec action ctx =
                    let act = getAction ctx
                    match act with
                    | Some fn ->
                        fn ()
                        action ctx
                    | None -> ()
                action context
            } |> Async.StartAsTask :> Task
        member this.Destroy() = ()
