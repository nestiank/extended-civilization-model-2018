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
            DefencePower = 15,
            GoldLogistics = 0,
            LaborLogistics = 0,
            MaxHealPerTurn = 20
        };

        public override void PostTurn()
        {
            if (Game.Random.Next(100) == 7)
            {
                SendUnit();
            }

            base.PostTurn();
        }

        private void SendUnit()
        {
            if (PlacedPoint is Terrain.Point thisPoint)
            {
                var creators = new Action<Terrain.Point>[] {
                    pt => DecentralizedMilitaryProductionFactory.Instance.Create(Owner).Place(pt),
                    pt => EMUHorseArcherProductionFactory.Instance.Create(Owner).Place(pt),
                    pt => ElephantCavalryProductionFactory.Instance.Create(Owner).Place(pt),
                    pt => AncientSorcererProductionFactory.Instance.Create(Owner).Place(pt),
                    pt => JediKnightProductionFactory.Instance.Create(Owner).Place(pt),
                };
                var creator = creators[Game.Random.Next(creators.Length)];

                foreach (var adjacent in thisPoint.Adjacents())
                {
                    if (adjacent is Terrain.Point pt && pt.Unit == null
                        && (pt.TileBuilding == null || pt.TileBuilding.Owner == Owner))
                    {
                        creator(pt);
                        return;
                    }
                }
            }
        }

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public FinnoEmpireCity(Player player, Terrain.Point point, bool isLoadFromFile)
            : base(player, Constants, point, null)
        {
            this.Population = 5;
            _specialActs[0] = new FinnoEmpireCityAction(this);

            if (!isLoadFromFile)
            {
                foreach (var pt in PlacedPoint.Value.Adjacents())
                {
                    if (pt.HasValue)
                        Owner.TryAddTerritory(pt.Value);
                }
            }
        }

        private class FinnoEmpireCityAction : IActorAction
        {
            private readonly FinnoEmpireCity _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public int LastSkillCalled = -1;

            public FinnoEmpireCityAction(FinnoEmpireCity owner)
            {
                _owner = owner;
            }

            private Exception CheckError(Terrain.Point? pt)
            {
                if (pt == null)
                    return new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    return new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber == LastSkillCalled)
                    return new InvalidOperationException("Skill is not turned on");
                if (pt.Value.Unit == null)
                    return new InvalidOperationException("There is no target");
                if (pt.Value.Unit.Owner == Owner.Owner)
                    return new InvalidOperationException("The Unit is not hostile");
                if (!this.IsInDistance(pt))
                    return new InvalidOperationException("Too Far to Attack");

                return null;
            }

            private bool IsInDistance(Terrain.Point? pt)
            {
                int A = Owner.PlacedPoint.Value.Position.A;
                int B = Owner.PlacedPoint.Value.Position.B;
                int C = Owner.PlacedPoint.Value.Position.C;
                int Width = Owner.PlacedPoint.Value.Terrain.Width;

                if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.A - A), Math.Abs(pt.Value.Position.B - B)), Math.Abs(pt.Value.Position.C - C)) > 2)
                {
                    if (pt.Value.Position.B > B) // pt가 맵 오른쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.B - Width - B), Math.Abs(pt.Value.Position.A + Width - A)), Math.Abs(pt.Value.Position.C - C)) > 2)
                            return false;
                    }
                    else //pt가 맵 왼쪽
                    {
                        if (Math.Max(Math.Max(Math.Abs(pt.Value.Position.B + Width - B), Math.Abs(pt.Value.Position.A - Width - A)), Math.Abs(pt.Value.Position.C - C)) > 2)
                            return false;
                    }
                }
                return true;
            }

            public ActionPoint GetRequiredAP(Terrain.Point? pt)
            {
                if (CheckError(pt) != null)
                    return double.NaN;

                return 0;
            }

            public void Act(Terrain.Point? pt)
            {
                if (CheckError(pt) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(pt);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");

                Owner.AttackTo(30, pt.Value.Unit, 0, false, true);

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
            }
        }

        protected override double CalculateDefencePower(double originalPower, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalPower + 15 * InteriorBuildings.OfType<AncientFinnoVigilant>().Count();
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

            return new FinnoEmpireCity(owner, point, false);
        }
    }
}
