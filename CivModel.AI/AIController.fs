namespace CivModel.AI

open System
open System.Linq;
open System.Threading.Tasks
open Accord.Fuzzy
open CivModel

type public AIController(player : Player) =
    let Research = new LinguisticVariable("Research", 0.f, infinityf)
    let ProductionLaborCost = new LinguisticVariable("ProductionLaborCost", 0.f, infinityf);
    let ProductionGoldCost = new LinguisticVariable("ProductionGoldCost", 0.f, infinityf);

    let ResearchLow = new FuzzySet("ResearchLow", new TrapezoidalFunction(0.f, 100.f, 200.f))
    let ResearchNormal = new FuzzySet("ResearchNormal", new TrapezoidalFunction(100.f, 200.f, 300.f))
    let ResearchHigh = new FuzzySet("ResearchHigh", new TrapezoidalFunction(200.f, 300.f, TrapezoidalFunction.EdgeType.Left))
    let CostLow = new FuzzySet("CostLow", new TrapezoidalFunction(0.f, 5.f, 10.f))
    let CostNormal = new FuzzySet("CostNormal", new TrapezoidalFunction(10.f, 15.f, 20.f))
    let CostHigh = new FuzzySet("CostHigh", new TrapezoidalFunction(20.f, 25.f, TrapezoidalFunction.EdgeType.Left))

    let BuildResearch = new LinguisticVariable("BuildResearch", -1.f, 1.f)
    let Production = new LinguisticVariable("Production", -1.f, 1.f)

    let VeryLow = new FuzzySet("VeryLow", new TrapezoidalFunction(-1.f, -0.8f, -0.6f))
    let Low = new FuzzySet("Low", new TrapezoidalFunction(-1.f, -0.6f, -0.2f))
    let Normal = new FuzzySet("Normal", new TrapezoidalFunction(-0.6f, -0.2f, 0.2f))
    let High = new FuzzySet("High", new TrapezoidalFunction(-0.2f, 0.6f, 1.0f))
    let VeryHigh = new FuzzySet("VeryHigh", new TrapezoidalFunction(0.6f, 0.8f, 1.0f))

    let fuzzyDB = new Database()
    let rules = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000))

    do
        Research.AddLabel(ResearchLow)
        Research.AddLabel(ResearchNormal)
        Research.AddLabel(ResearchHigh)
        fuzzyDB.AddVariable(Research)

        for var in [ ProductionLaborCost; ProductionGoldCost ] do
            var.AddLabel(CostLow)
            var.AddLabel(CostNormal)
            var.AddLabel(CostHigh)
            fuzzyDB.AddVariable(var);

        for var in [ BuildResearch; Production ] do
            var.AddLabel(VeryLow)
            var.AddLabel(Low)
            var.AddLabel(Normal)
            var.AddLabel(High)
            var.AddLabel(VeryHigh)
            fuzzyDB.AddVariable(var)

        let strRules = [
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
        strRules |> List.mapi (fun idx str -> rules.NewRule(idx.ToString(), str)) |> ignore

    let DoBuildResearch (x : float32) =
        let getPoint (p : IInteriorBuildingProductionFactory) =
            rules.SetInput(BuildResearch.Name, x)
            rules.SetInput(ProductionLaborCost.Name, float32 p.TotalLaborCost)
            rules.SetInput(ProductionGoldCost.Name, float32 p.TotalGoldCost)
            rules.Evaluate(Production.Name)
        let (factory, point) =
            player.AvailableProduction
            |> Enumerable.OfType<IInteriorBuildingProductionFactory>
            |> Seq.map (fun p -> (p, getPoint p)) |> Seq.maxBy snd
        if point > 0.f then
            player.Production.AddLast(factory.Create(player)) |> ignore
        else
            ()

    let actionMap = [ BuildResearch.Name, (DoBuildResearch, true) ] |> Map.ofSeq

    let getAction space =
        if Map.isEmpty space then
            None
        else
            rules.SetInput(Research.Name, float32 player.Research)
            let (k, v, x) =
                space |> Map.toSeq
                |> Seq.map (fun (k, v) -> (k, v, rules.Evaluate(k)))
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
