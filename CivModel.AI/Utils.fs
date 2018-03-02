namespace CivModel.AI

open CivModel

type Failable<'a, 'b> = Success of 'a | Fail of 'b

module Utils =
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
            match tbl with
            | (ddx, cond) :: xs -> 
                let pos = pt.Terrain.GetPoint (pt.Position + dx)
                if f pos then
                    Success pos
                else
                    let next = dx + ddx
                    if not (pt.Terrain.IsValidPosition next) then
                        circle next failval tbl
                    elif cond next then
                        circle next 1 tbl
                    else
                        circle next 1 xs
            | _ -> Fail failval
        let rec spread radius =
            match circle (Position.FromLogical (radius, -radius, 0)) -1 table with
            | Success x -> Some x
            | Fail -1 -> None
            | Fail _ -> spread (radius + 1)
        spread 1

    let fightUnits (player : Player) =
        player.Units |> Seq.filter (fun u -> u.MaxHP > 0.0)
