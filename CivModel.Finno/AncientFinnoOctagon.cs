using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoOctagon : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("AD9DD607-2972-40F7-8596-391955621CB3");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 20,
            DefencePower = 0,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 4
        };

        public AncientFinnoOctagon(Player owner, Terrain.Point point) : base(owner, Constants, point) { }

        public override void PostTurn()
        {
            base.PostTurn();

            Owner.Happiness += 2;
        }
    }

    public class AncientFinnoOctagonProductionFactory : ITileObjectProductionFactory
    {
        public static AncientFinnoOctagonProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoOctagonProductionFactory> _instance
            = new Lazy<AncientFinnoOctagonProductionFactory>(() => new AncientFinnoOctagonProductionFactory());
        private AncientFinnoOctagonProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoOctagon);
        public ActorConstants ActorConstants => AncientFinnoOctagon.Constants;

        public double TotalLaborCost => 20;
        public double LaborCapacityPerTurn => 15;
        public double TotalGoldCost => 60;
        public double GoldCapacityPerTurn => 20;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                 && !IsCityNeer(production, point)
                 && (point.TileOwner == production.Owner || point.TileOwner == production.Owner.Game.Players[3] || point.TileOwner == production.Owner.Game.Players[5] || point.TileOwner == production.Owner.Game.Players[7]);
        }

        private bool IsCityNeer(TileObjectProduction production, Terrain.Point point)
        {
            int A = point.Position.A;
            int B = point.Position.B;
            int C = point.Position.C;


            if (RealAction(A + 1, B - 1, C, production, point))
                return true;
            else if (RealAction(A + 1, B, C - 1, production, point))
                return true;
            else if (RealAction(A, B + 1, C - 1, production, point))
                return true;
            else if (RealAction(A - 1, B + 1, C, production, point))
                return true;
            else if (RealAction(A - 1, B, C + 1, production, point))
                return true;
            else if (RealAction(A, B - 1, C + 1, production, point))
                return true;

            else if (RealAction(A + 2, B - 2, C, production, point))
                return true;
            else if (RealAction(A + 2, B - 1, C - 1, production, point))
                return true;
            else if (RealAction(A + 2, B, C - 2, production, point))
                return true;
            else if (RealAction(A + 1, B + 1, C - 2, production, point))
                return true;
            else if (RealAction(A, B + 2, C - 2, production, point))
                return true;
            else if (RealAction(A - 1, B + 2, C - 1, production, point))
                return true;
            else if (RealAction(A - 2, B + 2, C, production, point))
                return true;
            else if (RealAction(A - 2, B + 1, C + 1, production, point))
                return true;
            else if (RealAction(A - 2, B, C + 2, production, point))
                return true;
            else if (RealAction(A - 1, B - 1, C + 2, production, point))
                return true;
            else if (RealAction(A, B - 2, C + 2, production, point))
                return true;
            else if (RealAction(A + 1, B - 2, C + 1, production, point))
                return true;

            return false;
        }

        private bool RealAction(int A, int B, int C, TileObjectProduction production, Terrain.Point point)
        {
            if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < point.Terrain.Width && 0 <= C && C < point.Terrain.Height)
            {
                if ((point.Terrain.GetPoint(A, B, C)).TileBuilding is CityBase)
                {
                    return true;
                }
            }
            return false;
        }

        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoOctagon(owner, point);
        }
    }
}
