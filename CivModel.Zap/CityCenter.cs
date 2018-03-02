using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public sealed class CityCenter : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("BF70E402-A0BE-4587-85A3-5623667AE2E1");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 30,
            GoldLogistics = 0,
            LaborLogistics = 0,
            MaxHealPerTurn = 30

        };


        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public CityCenter(Player player, Terrain.Point point) : base(player, Constants, point)
        {
            this.Population = 5;
            _specialActs[0] = new CityCenterAction(this);
        }

        private class CityCenterAction : IActorAction
        {
            private readonly CityCenter _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public int LastSkillCalled = -1;

            public CityCenterAction(CityCenter owner)
            {
                _owner = owner;
            }

            public double GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return double.NaN;
                if (!_owner.PlacedPoint.HasValue)
                    return double.NaN;
                if (Owner.Owner.Game.TurnNumber == LastSkillCalled)
                    return double.NaN;

                return 0;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber == LastSkillCalled)
                    throw new InvalidOperationException("Skill is not turned on");

                int A = Owner.PlacedPoint.Value.Position.A;
                int B = Owner.PlacedPoint.Value.Position.B;
                int C = Owner.PlacedPoint.Value.Position.C;

                RealAction(A + 1, B - 1, C);
                RealAction(A + 1, B, C - 1);
                RealAction(A, B + 1, C - 1);
                RealAction(A - 1, B + 1, C);
                RealAction(A - 1, B, C + 1);
                RealAction(A, B - 1, C + 1);

                RealAction(A + 2, B - 2, C);
                RealAction(A + 2, B - 1, C - 1);
                RealAction(A + 2, B, C - 2);
                RealAction(A + 1, B + 1, C - 2);
                RealAction(A, B + 2, C - 2);
                RealAction(A - 1, B + 2, C - 1);
                RealAction(A - 2, B + 2, C);
                RealAction(A - 2, B + 1, C + 1);
                RealAction(A - 2, B, C + 2);
                RealAction(A - 1, B - 1, C + 2);
                RealAction(A, B - 2, C + 2);
                RealAction(A + 1, B - 2, C + 1);

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
            }

            private void RealAction(int A, int B, int C)
            {
                if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < Owner.PlacedPoint.Value.Terrain.Width && 0 <= C && C < Owner.PlacedPoint.Value.Terrain.Height)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit.Owner != Owner.Owner)
                        {
                            Owner.AttackTo(30, (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit, 0, false, true);
                        }
                    }
                }
            }
        }

        protected override void OnProcessCreation()
        {
            base.OnProcessCreation();

            foreach (var pt in PlacedPoint.Value.Adjacents())
            {
                if (pt.HasValue)
                    Owner.TryAddTerritory(pt.Value);
            }
        }
    }

    public class CityCenterProductionFactory : ITileObjectProductionFactory
    {
        public static CityCenterProductionFactory Instance => _instance.Value;
        private static Lazy<CityCenterProductionFactory> _instance
            = new Lazy<CityCenterProductionFactory>(() => new CityCenterProductionFactory());

        private CityCenterProductionFactory()
        {
        }

        public ActorConstants Constants => CityCenter.Constants;

        public double TotalLaborCost => 100;
        public double LaborCapacityPerTurn => 25;
        public double TotalGoldCost => 200;
        public double GoldCapacityPerTurn => 50;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }

        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null
                && point.Unit is Pioneer pioneer
                && pioneer.Owner == production.Owner;
        }

        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            // remove pioneer
            point.Unit.Destroy();

            return new CityCenter(owner, point);
        }
    }
}
