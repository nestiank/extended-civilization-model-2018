namespace CivModel.AI

open System
open Accord.Fuzzy

type FuzzySystem() =
    let fuzzyDB = new Database()

    let trapezoidal m1 m2 m3 =
        if m1 = -infinityf then
            TrapezoidalFunction(m2, m3, TrapezoidalFunction.EdgeType.Right)
        elif m3 = infinityf then
            TrapezoidalFunction(m1, m2, TrapezoidalFunction.EdgeType.Left)
        else
            TrapezoidalFunction(m1, m2, m3)

    member this.CreateSetPoint3 name m1 m2 m3 = FuzzySet(name, trapezoidal m1 m2 m3)
    member this.CreateSetPoint4 name m1 m2 m3 m4 = FuzzySet(name, TrapezoidalFunction(m1, m2, m3, m4))
    member this.CreateSetSingleton name value = FuzzySet(name, SingletonFunction(value))

    member this.createSet name = function
        | [value] -> this.CreateSetSingleton name value
        | [m1; m2; m3] -> this.CreateSetPoint3 name m1 m2 m3
        | [m1; m2; m3; m4] -> this.CreateSetPoint4 name m1 m2 m3 m4
        | _ -> invalidArg "numPoints" "numPoints is invalid"

    member this.CreateSetsByLevel prefix names numPoints numSkip values =
        let rec create names values =
            match names with
                | n :: ns ->
                    let set = this.createSet (prefix + n) (List.take numPoints values)
                    set :: create ns (List.skip numSkip values)
                | _ -> []
        if numSkip < 1 then
            invalidArg "skip" "skip is not positive"
        else
            create names values

    member this.CreateSetsByList (dict : (string * (float32 list)) list) =
        dict |> List.map (fun (k, v) -> this.createSet k v)

    member this.CreateVariable name minval maxval sets =
        let var = LinguisticVariable(name, minval, maxval)
        sets |> Seq.iter var.AddLabel
        fuzzyDB.AddVariable(var)
        FuzzyVariable(this, var)

    member this.CreateRule rules =
        let r = FuzzyRules(fuzzyDB)
        r.AddRules rules
        r

and FuzzyRules(fuzzyDB : Database) =
    let mutable ruleNumber = 0
    let createRuleName() =
        let n = ruleNumber
        ruleNumber <- ruleNumber + 1
        sprintf "Rule %d" n

    member val InferSystem = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000))
    member this.AddRules rules =
        rules |> Seq.iter (fun rule -> this.InferSystem.NewRule(createRuleName(), rule) |> ignore)

and FuzzyVariable(system : FuzzySystem, var : LinguisticVariable) =
    member this.Name = var.Name

    member this.SetValue x (rules : FuzzyRules) =
        rules.InferSystem.SetInput(var.Name, x)
    member this.GetValue (rules : FuzzyRules) =
        rules.InferSystem.Evaluate var.Name

    override this.Equals other =
        match other with
            | :? FuzzyVariable as x -> this.Name = x.Name
            | _ -> false
    override this.GetHashCode() = this.Name.GetHashCode()
    interface IComparable with
        member this.CompareTo other =
            match other with
                | :? FuzzyVariable as x -> this.Name.CompareTo x.Name
                | _ -> invalidArg "other" "Tried to compare incompatible types"
