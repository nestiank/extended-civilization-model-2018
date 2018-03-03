namespace CivModel.AI

open System
open System.Linq
open CivModel
open CivModel.Common

open AIFuzzyModule

type MoveRules(player : Player, rules : FuzzyRules) =
    let getEnemy (pt : Terrain.Point) =
        if pt.TileBuilding <> null && pt.TileBuilding.MaxHP > 0.0 then pt.TileBuilding :> Actor
        else pt.Unit :> Actor

    let adjEnemy (pt : Terrain.Point) =
        pt.Adjacents() |> Array.map Option.ofNullable
        |> Array.choose (function Some adj -> Option.ofObj (getEnemy adj) | None -> None)

    let tryGetScore (unit : Unit) p =
        match Option.ofNullable p with
        | Some adj ->
            let setCommon (action : IActorAction) =
                rules |> MyUnit.SetValue (if unit.MaxHP > 0.0 then 0.f else 1.f)

                let nowEnemies = adjEnemy unit.PlacedPoint.Value
                let targetEnemies = adjEnemy adj

                let dmgFilter (x : Actor) = action.IsActable unit.PlacedPoint
                let dmg =
                    (targetEnemies |> Array.filter dmgFilter |> Array.sumBy (fun x -> x.AttackPower))
                    - (nowEnemies |> Array.filter dmgFilter |> Array.sumBy (fun x -> x.AttackPower))
                rules |> SpotDamage.SetValue (float32 (dmg / (unit.MaxHP + 1.0)))

                let dealFilter (x : Actor) = action.IsActable x.PlacedPoint
                let deal =
                    (targetEnemies |> Array.filter dealFilter |> Array.length)
                    - (nowEnemies |> Array.filter dealFilter |> Array.length)
                rules |> SpotDeal.SetValue (float32 deal)

                let distFilter (x : Actor) = x.Owner <> player && (x :? Unit || x :? CityBase)
                let (_, nowEnemDist) = Utils.getNearestActor unit.PlacedPoint.Value distFilter player
                let (_, targetEnemDist) = Utils.getNearestActor adj distFilter player
                rules |> SpotEnemDist.SetValue (float32 (targetEnemDist - nowEnemDist))

                let cityFilter (x : Actor) = x.Owner = player && x :? CityBase
                let (_, nowCityDist) = Utils.getNearestActor unit.PlacedPoint.Value cityFilter player
                let (_, targetCityDist) = Utils.getNearestActor adj cityFilter player
                rules |> SpotMyCityDist.SetValue (float32 (targetCityDist - nowCityDist))
                    
            let retCommon (action : IActorAction) =
                let score = rules |> MoveUnittoSpot.GetValue
                if score > 0.f then Some (score, action, adj)
                else None

            if unit.MoveAct <> null && unit.MoveAct.IsActable (Nullable adj) then
                setCommon unit.MoveAct
                rules |> MyUnitHP.SetValue (float32 (unit.RemainHP / unit.MaxHP))
                rules |> EnemyUnitHP.SetValue 1.0f
                rules |> EnemyCityHP.SetValue 1.0f
                retCommon unit.MoveAct
            elif unit.MovingAttackAct <> null && unit.MovingAttackAct.IsActable (Nullable adj) then
                let enemy =
                    if adj.TileBuilding <> null && adj.TileBuilding.Owner <> player && adj.TileBuilding.MaxHP > 0.0 then adj.TileBuilding :> Actor
                    elif adj.Unit <> null && adj.Unit.Owner <> null then adj.Unit :> Actor
                    else null
                let myHP = max 0.0 (unit.RemainHP - enemy.DefencePower)
                let enemyHP = max 0.0 (enemy.RemainHP - unit.AttackPower)
                setCommon unit.MovingAttackAct
                rules |> MyUnitHP.SetValue (float32 (myHP / unit.MaxHP))
                rules |> EnemyUnitHP.SetValue (float32 enemyHP)
                rules |> EnemyCityHP.SetValue (if enemy :? CityBase then float32 enemyHP else 1.0f)
                retCommon unit.MoveAct
            else
                None
        | None -> None

    let rec doFuzzyAction() =
        let moveAction (unit : Unit) =
            match Option.ofNullable unit.PlacedPoint with
            | Some pt ->
                match pt.Adjacents() |> Array.choose (tryGetScore unit) with
                | [||] -> false
                | ar ->
                    let (_, action, param) = ar |> Array.maxBy (fun (s, _, _) -> s) 
                    action.Act (Nullable param)
                    true
            | None -> false
        let rs =  player.Units |> Seq.map moveAction
        if Seq.isEmpty rs || Seq.forall ((=) false) rs then
            ()
        else
            doFuzzyAction()

    member val FuzzyRules = rules
    member this.DoFuzzyAction = doFuzzyAction
