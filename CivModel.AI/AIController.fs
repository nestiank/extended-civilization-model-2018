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
        Movement.getAction;
    ]

    let getAction (context: AIContext) =
        context.Update ()
        actions |> List.tryPick ((|>) context)

    interface CivModel.IAIController with
        member this.DoAction() =
            let rec action ctx =
                let act = getAction ctx
                match act with
                | Some fn ->
                    fn ()
                    action ctx
                | None -> ()
            action (AIContext player)
            Task.CompletedTask
        member this.Destroy() = ()
