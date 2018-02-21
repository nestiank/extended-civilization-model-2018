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
    let rec doDeploy() =
        seq {
            match (player.Deployment |> Seq.toList) with
                | x :: xs ->
                    yield deploy x
                    yield! doDeploy()
                | [] -> ()
        }

    let rec doAction' lst =
        let do_one s =
            if Seq.isEmpty s then s, false
            else (s |> Seq.skip 1), true
        let next = lst |> List.map do_one
        if next |> List.filter snd |> List.isEmpty then
            false
        else
            doAction' (next |> List.map fst) |> ignore
            true
    let doAction lst =
        let rec foo retry =
            if doAction' (lst |> List.map (fun f -> f() |> Seq.cache)) then foo false
            elif not retry then foo true
            else ()
        foo false

    let mutable prevResearch = -infinity
    let mutable prevLabor = -infinity
    interface CivModel.IAIController with
        member this.DoAction() =
            async {
                doAction [ doDeploy; fuzzyRules.DoFuzzyAction ]

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
