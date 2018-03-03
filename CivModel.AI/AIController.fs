namespace CivModel.AI

open System
open System.Linq
open System.Threading.Tasks
open CivModel

type asdfasdf(player : Player) =
    let mutable counter = 0
    interface ITurnObserver with
        member this.PreTurn () = ()
        member this.PostTurn () =
            if counter = 10 then
                let q = player.Quests.FirstOrDefault ()
                if q = null then ()
                else q.Complete()
                counter <- 0
            else
                counter <- counter + 1
        member this.PrePlayerSubTurn p = ()
        member this.PostPlayerSubTurn p = ()

type public AIController(player : Player) =
    let globalRules = GlobalRules(player)
    let moveRules = MoveRules(player, globalRules.FuzzyRules)

    let deploy (x : Production) =
        match x.Factory with
        | :? IInteriorBuildingProductionFactory ->
            let city' = player.Cities |> Seq.tryFind (fun c -> c.PlacedPoint.HasValue && x.IsPlacable c.PlacedPoint.Value)
            match city' with
            | Some city ->
                player.Deployment.Remove x |> ignore
                x.Place city.PlacedPoint.Value
            | None -> ()
        | :? ITileObjectProductionFactory ->
            let pt' = player.Game.Terrain.AllTiles |> Seq.tryFind (fun pt -> x.IsPlacable pt)
            match pt' with
            | Some pt ->
                player.Deployment.Remove x |> ignore
                x.Place pt
            | None -> ()
        | _ ->
            System.Diagnostics.Debug.WriteLine "unqualified production in AIController.deploy"
            ()
    let rec doDeploy' = function
        | x :: xs ->
            deploy x
            doDeploy' xs
        | [] -> ()
    let doDeploy() = doDeploy' (Seq.toList player.Deployment)

    let mutable prevResearch = -infinity
    let mutable prevLabor = -infinity

    interface CivModel.IAIController with
        member this.DoAction() =
            async {
                globalRules.DoFuzzyAction()
                moveRules.DoFuzzyAction()
                doDeploy()

                let researchDiff = player.Research - prevResearch
                let laborDiff = player.Labor - prevLabor
                prevResearch <- player.Research
                prevLabor <- player.Labor

                let msg =
                    sprintf "Research[ %f, diff: %f ] Labor[ %f, diff: %f ]"
                        player.Research researchDiff
                        player.Labor laborDiff
                System.Diagnostics.Debug.WriteLine msg
            } |> Async.StartAsTask :> Task
        member this.Destroy() = ()
