namespace CivModel.AI

open Accord.Fuzzy
open CivModel
open System
open System.Collections.Generic;
open System.Threading.Tasks

type public AIController(player : Player) =
    let ResearchLow = new FuzzySet("ResearchLow", new TrapezoidalFunction(100.f, 200.f, TrapezoidalFunction.EdgeType.Right))

    let Research = new LinguisticVariable("Research", 0.f, infinityf)

    let VeryLow = new FuzzySet("VeryLow", new TrapezoidalFunction(-0.1f, 0.1f, TrapezoidalFunction.EdgeType.Left))
    let Low = new FuzzySet("Low", new TrapezoidalFunction(0.1f, 0.3f, TrapezoidalFunction.EdgeType.Left))
    let Normal = new FuzzySet("Normal", new TrapezoidalFunction(0.3f, 0.5f, TrapezoidalFunction.EdgeType.Left))
    let High = new FuzzySet("High", new TrapezoidalFunction(0.5f, 0.7f, TrapezoidalFunction.EdgeType.Left))
    let VeryHigh = new FuzzySet("VeryHigh", new TrapezoidalFunction(0.7f, 0.9f, TrapezoidalFunction.EdgeType.Left))

    let BuildResearch = new LinguisticVariable("BuildResearch", -1.f, 1.f)

    let fuzzyDB = new Database()
    let rules = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000))

    do
        Research.AddLabel(ResearchLow)

        for var in [ BuildResearch ] do
            var.AddLabel(VeryLow)
            var.AddLabel(Low)
            var.AddLabel(Normal)
            var.AddLabel(High)
            var.AddLabel(VeryHigh)

        fuzzyDB.AddVariable(Research)
        fuzzyDB.AddVariable(BuildResearch)

        rules.NewRule("1", "IF Research is ResearchLow THEN BuildResearch is High") |> ignore

    let DoBuildResearch (x : float32) =
        player.ResearchInvestmentRatio <- float x

    let actionMap = [ BuildResearch.Name, DoBuildResearch ] |> Map.ofSeq

    let inference (space : Map<string, (float32 -> unit)>) =
        if Map.isEmpty space then
            None
        else
            rules.SetInput("Research", float32 player.Research)
            Some (space |> Map.toSeq
                |> Seq.map (fun (k, v) -> (k, v, rules.Evaluate(k)))
                |> Seq.maxBy (fun (_, _, x) -> x))

    let rec doJobs (space : Map<string, (float32 -> unit)>) =
        match inference space with
            | Some (k, v, x) ->
                v x
                doJobs (space |> Map.remove k)
            | None -> ()

    interface CivModel.IAIController with
        member this.DoAction() = async { doJobs actionMap } |> Async.StartAsTask :> Task
        member this.Destroy() = ()
