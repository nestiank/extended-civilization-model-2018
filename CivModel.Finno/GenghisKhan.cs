using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class GenghisKhan : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("DCB724DA-430E-4B13-8703-9517F173FE4B");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxAP = 2,
            MaxHP = 50,
            AttackPower = 35,
            DefencePower = 10,
            GoldLogistics = 50,
            FullLaborForRepair = 3,
            BattleClassLevel = 4
        };

        public override ActionPoint GetRequiredAPToMoveNearBy(Terrain.Point from, Terrain.Point to)
        {
            // POMFSTIH
            switch (to.Type)
            {
                case TerrainType.Plain: return 1;
                case TerrainType.Ocean:
                    if (from.Type != TerrainType.Ocean)
                        return new ActionPoint(0.5, consumeAll: true);
                    else
                        return 0.5;
                case TerrainType.Mount: return 1;
                case TerrainType.Forest: return 1;
                case TerrainType.Swamp: return 1;
                case TerrainType.Tundra: return 1;
                case TerrainType.Ice: return 1;
                case TerrainType.Hill: return 1;
                default: throw new NotImplementedException("unqualified TerrainType");
            }
        }

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public GenghisKhan(Player owner, Terrain.Point point) : base(owner, Constants, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new GenghisKhanAction(this);
        }

        private class GenghisKhanAction : IActorAction
        {
            private readonly GenghisKhan _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public GenghisKhanAction(GenghisKhan owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -2;

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target != null)
                    return new ArgumentException("pt is invalid");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 1)
                    return new InvalidOperationException("Skill is not turned on");
                if (Owner.PlacedPoint.Value.TileOwner == Owner.Owner)
                    return new InvalidOperationException("You have the Territory");

                return null;
            }

            public void Act(Terrain.Point? target)
            {
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                var origin = _owner.PlacedPoint.Value;

                if (CheckError(origin, target) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(origin, target);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");

                Owner.Owner.AddTerritory(Owner.PlacedPoint.Value);
                if (Owner.PlacedPoint.Value.TileBuilding != null)
                {
                    Owner.PlacedPoint.Value.TileBuilding.Destroy();
                }

                new AncientFinnoFIRFortress(Owner.Owner, Owner.PlacedPoint.Value);

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }
        }


        public override void PostTurn()
        {
            base.PostTurn();

            int GetUnit = Game.Random.Next(100);

            if (GetUnit <= 20)
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
            int PointB = B;
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

            if (IsItOk)
            {
                new EMUHorseArcher(Owner, this.PlacedPoint.Value.Terrain.GetPoint(PointA, PointB, PointC));

            }
        }

        private bool CheckUnit(int A, int B, int C)
        {
            int Width = this.PlacedPoint.Value.Terrain.Width;

            if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < Width && 0 <= C && C < this.PlacedPoint.Value.Terrain.Height)
            {
                if (this.PlacedPoint.Value.Terrain.GetPoint(A, B, C).Unit == null)
                {
                    if (this.PlacedPoint.Value.Terrain.GetPoint(A, B, C).TileBuilding != null)
                    {
                        if (this.PlacedPoint.Value.Terrain.GetPoint(A, B, C).TileBuilding.Owner != Owner)
                            return true;

                        else
                            return false;
                    }

                    else
                        return false;
                }                    

                else
                    return true;
            }

            else if (B + (C + Math.Sign(C)) / 2 >= Width)
            {
                if (this.PlacedPoint.Value.Terrain.GetPoint(A + Width, B - Width, C).Unit == null)
                {
                    if (this.PlacedPoint.Value.Terrain.GetPoint(A + Width, B - Width, C).TileBuilding != null)
                    {
                        if (this.PlacedPoint.Value.Terrain.GetPoint(A + Width, B - Width, C).TileBuilding.Owner != Owner)
                            return true;

                        else
                            return false;
                    }

                    else
                        return false;
                }

                else
                    return true;
            }

            else if(0 > B + (C + Math.Sign(C)) / 2)
            {
                if (this.PlacedPoint.Value.Terrain.GetPoint(A - Width, B + Width, C).Unit == null)
                {
                    if (this.PlacedPoint.Value.Terrain.GetPoint(A - Width, B + Width, C).TileBuilding != null)
                    {
                        if (this.PlacedPoint.Value.Terrain.GetPoint(A - Width, B + Width, C).TileBuilding.Owner != Owner)
                            return true;

                        else
                            return false;
                    }

                    else
                        return false;
                }

                else
                    return true;
            }

            return true;
        }
    }

    public class GenghisKhanProductionFactory : IActorProductionFactory
    {
        private static Lazy<GenghisKhanProductionFactory> _instance
            = new Lazy<GenghisKhanProductionFactory>(() => new GenghisKhanProductionFactory());
        public static GenghisKhanProductionFactory Instance => _instance.Value;
        private GenghisKhanProductionFactory()
        {
        }

        public Type ResultType => typeof(GenghisKhan);
        public ActorConstants ActorConstants => GenghisKhan.Constants;

        public double TotalLaborCost => 100;
        public double LaborCapacityPerTurn => 20;
        public double TotalGoldCost => 100;
        public double GoldCapacityPerTurn => 10;

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityBase
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new GenghisKhan(owner, point);
        }
    }
}
