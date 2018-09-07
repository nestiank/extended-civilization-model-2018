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
