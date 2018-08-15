namespace CivModel.AI

open System
open System.Linq
open System.Threading.Tasks
open CivModel

type AIController(player : Player) =
    let fallback fn = function
    | Some x -> Some x
    | None -> fn ()

    let getAction () =
        let context = AIContext player
        None
        |> Option.orElseWith (fun _ -> Produce.getAction context)
        |> Option.orElseWith (fun _ -> Deploy.getAction context)
        |> Option.orElseWith (fun _ -> Movement.getAction context)

    interface CivModel.IAIController with
        member this.DoAction() =
            let rec action () =
                let act = getAction ()
                match act with
                | Some fn ->
                    fn ()
                    action ()
                | None -> ()
            action ()
            Task.CompletedTask
        member this.Destroy() = ()
