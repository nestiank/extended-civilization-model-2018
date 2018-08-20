namespace CivModel.AI

open System
open CivModel

module Battle =
    let availableAttackAct (unit: Unit) (target: Terrain.Point) =
        if unit.MovingAttackAct <> null && unit.MovingAttackAct.IsActable (Nullable target) then
            Some (unit.MovingAttackAct, target)
        elif unit.HoldingAttackAct <> null && unit.HoldingAttackAct.IsActable (Nullable target) then
            Some (unit.HoldingAttackAct, target)
        else None

    let battleAction (context: AIContext) (unit: Unit) =
        let available =
            unit.PlacedPoint.Value.Adjacents ()
            |> Array.choose (fun x -> x |> Option.ofNullable |> Option.bind (availableAttackAct unit))
            |> Array.tryHead
        match available with
        | Some (act, target) ->
            Some (fun () -> act.Act (Nullable target))
        | None -> None

    let getAction (context: AIContext) =
        context.Player.Units |> Seq.tryPick (battleAction context)
