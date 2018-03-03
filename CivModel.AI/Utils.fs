namespace CivModel.AI

open System.Linq
open CivModel

type Failable<'a, 'b> = Success of 'a | Fail of 'b

module Utils =
    [<Literal>]
    let HwanPlayer = CivModel.Hwan.HwanPlayerConstant.HwanPlayer
    [<Literal>]
    let FinnoPlayer = CivModel.Finno.FinnoPlayerConstant.FinnoPlayer

    let searchNear f (pt : Terrain.Point) =
        let table : (Position * (Position -> bool)) list = [
                Position.FromLogical(0, 1, -1), fun p -> p.B < 0;
                Position.FromLogical(-1, 1, 0), fun p -> p.A > 0;
                Position.FromLogical(-1, 0, 1), fun p -> p.C < 0;
                Position.FromLogical(0, -1, 1), fun p -> p.B > 0;
                Position.FromLogical(1, -1, 0), fun p -> p.A < 0;
                Position.FromLogical(1, 0, -1), fun p -> p.C > 0;
            ]
        let rec circle dx failval tbl =
            let isValid (pos : Position) =
                let (w, h) = pt.Terrain.Width, pt.Terrain.Height
                -w <= pos.X && pos.X < 2 * w && 0 <= pos.Y && pos.Y < h
            match tbl with
            | (ddx, cond) :: xs -> 
                let pos = pt.Position + dx
                let (valid, nextfail) =
                    if isValid pos then true, 1
                    else false, failval
                if valid && f (pt.Terrain.GetPoint pos) then
                    Success pos
                else
                    let next = dx + ddx
                    if cond next then
                        circle next nextfail tbl
                    else
                        circle next nextfail xs
            | _ -> Fail failval
        let rec spread radius =
            match circle (Position.FromLogical (radius, -radius, 0)) -1 table with
            | Success x -> Some (pt.Terrain.GetPoint x)
            | Fail -1 -> None
            | Fail _ -> spread (radius + 1)
        spread 1

    let getNearestActor (pt : Terrain.Point) f (player : Player) =
        player.Game.Players
        |> Seq.collect (fun p -> p.Actors)
        |> Seq.filter (fun a -> a.PlacedPoint.HasValue && f a)
        |> Seq.map (fun a -> a, Terrain.Point.Distance (a.PlacedPoint.Value, pt))
        |> Seq.minBy snd

    let fightUnitsOfPlayer (player : Player) =
        player.Units |> Seq.filter (fun u -> u.MaxHP > 0.0)
    let fightUnitsOfProducts (prods : Production seq) =
        prods |> Seq.filter (fun p ->
            match p.Factory with
            | :? IActorProductionFactory as f ->
                if typeof<Unit>.IsAssignableFrom f.ResultType && f.ActorConstants.MaxHP > 0.0 then
                    true
                else false
            | _ -> false)

    let productsOfType<'a> (prods : Production seq) =
        prods |> Seq.filter (fun p -> typeof<'a>.IsAssignableFrom p.Factory.ResultType)
