namespace CivModel.AI

open System
open System.Linq;
open System.Threading.Tasks
open Accord.Fuzzy
open CivModel

type public AIController(player : Player) =
    let system = FuzzySystem()

    let ResearchSets = system.CreateSetsByLevel "Research" [ "Low"; "Normal"; "High" ] 1 [ 0.f; 100.f; 200.f; 300.f; infinityf ]
    let Research = system.CreateVariable "Research" 0.f infinityf ResearchSets

    let CostSets = system.CreateSetsByLevel "Cost" [ "Low"; "Normal"; "High" ] 2 [ 0.f; 5.f; 10.f; 15.f; 20.f; 25.f; infinityf ]
    let ProductionLaborCost = system.CreateVariable "ProductionLaborCost" 0.f infinityf CostSets
    let ProductionGoldCost = system.CreateVariable "ProductionGoldCost" 0.f infinityf CostSets

    let ResultSets = system.CreateSetsByLevel "" [ "VeryLow"; "Low"; "Normal"; "High"; "VeryHigh" ] 1 [ -1.0f; -0.8f; -0.6f; -0.2f; 0.2f; 0.8f; 1.0f ]
    let BuildResearch = system.CreateVariable "BuildResearch" -1.f 1.f ResultSets
    let Production = system.CreateVariable "Production" -1.f 1.f ResultSets

    let rules = [
        "IF Research is ResearchLow THEN BuildResearch is High";
        "IF Research is ResearchNormal THEN BuildResearch is Normal";
        "IF Research is ResearchHigh THEN BuildResearch is VeryLow";
        "IF BuildResearch is High AND ProductionLaborCost is CostLow AND ProductionGoldCost is CostLow THEN Production is VeryHigh";
        "IF BuildResearch is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostLow THEN Production is High";
        "IF BuildResearch is High AND ProductionLaborCost is CostLow AND ProductionGoldCost is CostNormal THEN Production is High";
        "IF BuildResearch is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostNormal THEN Production is Normal";
        "IF BuildResearch is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostHigh THEN Production is Low";
        "IF BuildResearch is High AND ProductionLaborCost is CostHigh AND ProductionGoldCost is CostNormal THEN Production is Low";
        "IF BuildResearch is High AND ProductionLaborCost is CostHigh AND ProductionGoldCost is CostHigh THEN Production is VeryLow";
        ]

    do
        system.AddRules rules

    let DoBuildResearch (x : float32) =
        let getPoint (p : IInteriorBuildingProductionFactory) =
            BuildResearch.SetValue x
            ProductionLaborCost.SetValue (float32 p.TotalLaborCost)
            ProductionGoldCost.SetValue (float32 p.TotalGoldCost)
            Production.GetValue()
        let (factory, point) =
            player.AvailableProduction
            |> Enumerable.OfType<IInteriorBuildingProductionFactory>
            |> Seq.map (fun p -> (p, getPoint p)) |> Seq.maxBy snd
        if point > 0.f then
            player.Production.AddLast(factory.Create(player)) |> ignore
        else
            ()

    let actionMap = [ BuildResearch, (DoBuildResearch, true) ] |> Map.ofSeq

    let getAction (space : Map<FuzzyVariable, (float32 -> unit) * bool>) =
        if Map.isEmpty space then
            None
        else
            Research.SetValue (float32 player.Research)
            let (k, v, x) =
                space |> Map.toSeq
                |> Seq.map (fun (k, v) -> (k, v, k.GetValue()))
                |> Seq.maxBy (fun (_, _, x) -> x)
            if x > 0.f then Some (k, v, x) else None

    let rec doAction space =
        match getAction space with
            | Some (k, v, x) ->
                (fst v) x
                let nextSpace =
                    if snd v then space |> Map.remove k else space
                doAction nextSpace
            | None -> ()

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

    interface CivModel.IAIController with
        member this.DoAction() =
            async {
                deployList (player.Deployment |> Seq.toList)
                doAction actionMap
                System.Diagnostics.Debug.WriteLine(player.Research)
            } |> Async.StartAsTask :> Task
        member this.Destroy() = ()
