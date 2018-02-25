namespace CivModel.AI

open CivModel

type public AIController(player : Player) =
    interface CivModel.IAIController with
        member this.DoAction() = System.Threading.Tasks.Task.Delay 3000
        member this.Destroy() = ()
