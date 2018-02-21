namespace CivModel.AI

open System
open System.Threading.Tasks
open CivModel

type public AIController(player : Player) =
    let fuzzyRules = AIFuzzyModule.Rules(player)

    let deploy (x : Production) =
        match x.Factory with
            | :? IInteriorBuildingProductionFactory ->
                let city' = player.Cities |> Seq.tryFind (fun c -> c.PlacedPoint.HasValue && x.IsPlacable(c.PlacedPoint.Value))
                match city' with
                    | Some city ->
                        player.Deployment.Remove(x) |> ignore
                        x.Place(city.PlacedPoint.Value)
                    | None -> ()
            | _ -> ()
    let rec deployList : (Production list -> unit) = function
        | x :: xs -> deploy x; deployList xs
        | [] -> ()
    
    let mutable prevResearch = -infinity
    let mutable prevLabor = -infinity
    interface CivModel.IAIController with
        member this.DoAction() =
            async {
                deployList (player.Deployment |> Seq.toList)
                fuzzyRules.DoFuzzyAction()

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
