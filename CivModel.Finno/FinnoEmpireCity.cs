using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class FinnoEmpireCity : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("300E06FD-B656-46DC-A668-BB36C75E3086");
        public override Guid Guid => ClassGuid;


        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 500,
            GoldLogistics = 0,
            LaborLogistics = 0,
            MaxHealPerTurn = 20
        };

        public override void PostTurn()
        {
            base.PostTurn();
            Random r = new Random();

            int GetUnit = r.Next(1, 100);

            if (GetUnit <= 10)
            {
                SendUnit(GetUnit);
            }
        }

        private void SendUnit(int rand)
        {
            int A = this.PlacedPoint.Value.Position.A;
            int B = this.PlacedPoint.Value.Position.B;
            int C = this.PlacedPoint.Value.Position.C;

            bool IsItOk = false;

            int PointA = A;
            int PointB = B ;
            int PointC = C;

            if (!CheckUnit(A + 1, B - 1, C))
            {
                IsItOk = true;

                PointA = A + 1;
                PointB = B - 1;
                PointC = C;
            }

            else if (!CheckUnit(A + 1, B, C - 1))
            {
                IsItOk = true;

                PointA = A + 1;
                PointB = B;
                PointC = C - 1;
            }

            else if (!CheckUnit(A, B + 1, C - 1))
            {
                IsItOk = true;

                PointA = A;
                PointB = B + 1;
                PointC = C - 1;
            }

            else if (!CheckUnit(A - 1, B + 1, C))
            {
                IsItOk = true;

                PointA = A - 1;
                PointB = B + 1;
                PointC = C;
            }
            
            else if (!CheckUnit(A - 1, B, C + 1))
            {
                IsItOk = true;

                PointA = A - 1;
                PointB = B;
                PointC = C + 1;
            }

            else if (!CheckUnit(A, B - 1, C + 1))
            {
                IsItOk = true;

                PointA = A;
                PointB = B - 1;
                PointC = C + 1;
            }

            var ppt = this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC);
            if (ppt.Unit == null && ppt.TileBuilding == null)
            {
                if (IsItOk)
                {
                    if (rand <= 2)
                        new DecentralizedMilitary(Owner, this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));


                    else if (rand <= 4)
                        new EMUHorseArcher(Owner, this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));


                    else if (rand <= 6)
                        new ElephantCavalry(Owner, this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));

                    else if (rand <= 8)
                        new AncientSorcerer(Owner, this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));

                    else
                        new JediKnight(Owner, this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));
                }
            }
        }

        private bool CheckUnit (int A, int B, int C)
        {
            if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < this.PlacedPoint.Value.Terrain.Width && 0 <= C && C < this.PlacedPoint.Value.Terrain.Height)
            {
                if ((this.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).Unit != null && (this.PlacedPoint.Value.Terrain.GetPoint(A, B, C)).TileOwner == this.Owner)
                    return false;

                else
                    return true;
            }

            return true;
        }

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public FinnoEmpireCity(Player player, Terrain.Point point) : base(player, Constants, point)
        {
            this.Population = 5;
            _specialActs[0] = new FinnoEmpireCityAction(this);
        }

        private class FinnoEmpireCityAction : IActorAction
        {
            private readonly FinnoEmpireCity _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public int LastSkillCalled = -1;

            public FinnoEmpireCityAction(FinnoEmpireCity owner)
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

        protected override double CalculateDamage(double originalDamage, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            double damage = 15 + 15 * InteriorBuildings.OfType<AncientFinnoVigilant>().Count();
            AttackTo(damage, opposite, opposite.DefencePower, false, true);
            return originalDamage;
        }
    }

    public class FinnoEmpireCityProductionFactory : ITileObjectProductionFactory
    {
        public static FinnoEmpireCityProductionFactory Instance => _instance.Value;
        private static Lazy<FinnoEmpireCityProductionFactory> _instance
            = new Lazy<FinnoEmpireCityProductionFactory>(() => new FinnoEmpireCityProductionFactory());

        private FinnoEmpireCityProductionFactory()
        {
        }

        public Type ResultType => typeof(FinnoEmpireCity);
        public ActorConstants Constants => FinnoEmpireCity.Constants;

        public double TotalLaborCost => 200;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 300;
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

            return new FinnoEmpireCity(owner, point);
        }
    }
}
