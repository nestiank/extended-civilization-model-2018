namespace CivModel.AI

open System
open System.Linq;
open System.Threading.Tasks
open Accord.Fuzzy
open CivModel

type FuzzySystem() =
    let fuzzyDB = new Database()
    let inferSystem = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000))
    let mutable ruleNumber = 0

    let trapezoidal m1 m2 m3 =
        if m1 = -infinityf then
            new TrapezoidalFunction(m2, m3, TrapezoidalFunction.EdgeType.Right)
        elif m3 = infinityf then
            new TrapezoidalFunction(m1, m2, TrapezoidalFunction.EdgeType.Left)
        else
            new TrapezoidalFunction(m1, m2, m3)

    member val InferSystem = inferSystem

    member this.CreateSet name m1 m2 m3 =
        new FuzzySet(name, trapezoidal m1 m2 m3)

    member this.CreateSetsByLevel prefix names skip values =
        let rec create names values =
            match (names, values) with
                | (n :: ns, m1 :: m2 :: m3 :: _) ->
                    let set = this.CreateSet (prefix + n) m1 m2 m3
                    set :: create ns (values |> List.skip skip)
                | ([], _) -> []
                | _ -> invalidArg "names" "names and values are not corresponding"
        if skip < 1 then
            invalidArg "skip" "skip is not positive"
        else
            create names values

    member this.CreateVariable name minval maxval sets =
        let var = LinguisticVariable(name, minval, maxval)
        sets |> Seq.iter var.AddLabel
        fuzzyDB.AddVariable(var)
        FuzzyVariable(this, var)

    member this.AddRules rules =
        rules |> Seq.iter (fun rule ->
            inferSystem.NewRule(ruleNumber.ToString(), rule) |> ignore
            ruleNumber <- ruleNumber + 1)

and FuzzyVariable(system : FuzzySystem, var : LinguisticVariable) =
    member this.Name = var.Name

    member this.SetValue x =
        system.InferSystem.SetInput(var.Name, x)
    member this.GetValue() =
        system.InferSystem.Evaluate(var.Name)

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
