using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class GenghisKhan : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("DCB724DA-430E-4B13-8703-9517F173FE4B");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 50;

        public override double AttackPower => 35;
        public override double DefencePower => 10;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public GenghisKhan(Player owner, Terrain.Point point) : base(owner, point)
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

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;
                if (Owner.Owner.Game.TurnNumber < LastSkillCalled + 2)
                    return -1;

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber < LastSkillCalled + 5)
                    throw new InvalidOperationException("Skill is not turned on");
                if (Owner.PlacedPoint.Value.TileOwner == Owner.Owner)
                    throw new InvalidOperationException("You have the Territory");

                Owner.Owner.AddTerritory(Owner.PlacedPoint.Value);
                if (Owner.PlacedPoint.Value.TileBuilding != null)
                {
                    Owner.PlacedPoint.Value.TileBuilding.Destroy();
                }

            }
        }
    }

    public class GenghisKhanProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<GenghisKhanProductionFactory> _instance
            = new Lazy<GenghisKhanProductionFactory>(() => new GenghisKhanProductionFactory());
        public static GenghisKhanProductionFactory Instance => _instance.Value;
        private GenghisKhanProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 100, 20, 50, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new GenghisKhan(owner, point);
        }
    }
}
