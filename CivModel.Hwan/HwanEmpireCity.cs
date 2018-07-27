using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public sealed class HwanEmpireCity : CityBase
    {
        public static Guid ClassGuid { get; } = new Guid("D0A84907-885A-44C2-8E4C-077744E1E0C3");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 500,
            DefencePower = 15,
            GoldLogistics = 0,
            LaborLogistics = 0,
            MaxHealPerTurn = 20
        };


        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public HwanEmpireCity(Player player, Terrain.Point point, bool isLoadFromFile)
            : base(player, Constants, point, null)
        {
            this.Population = 5;
            _specialActs[0] = new HwanEmpireCityAction(this);

            if (!isLoadFromFile)
            {
                foreach (var pt in PlacedPoint.Value.Adjacents())
                {
                    if (pt.HasValue)
                        Owner.TryAddTerritory(pt.Value);
                }
            }
        }

        private class HwanEmpireCityAction : IActorAction
        {
            private readonly HwanEmpireCity _owner;
            public Actor Owner => _owner;

            public bool IsParametered => true;

            public int LastSkillCalled = -1;

            public HwanEmpireCityAction(HwanEmpireCity owner)
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

        protected override void OnDie(Player opposite)
        {
            Owner.Gold += 200*InteriorBuildings.OfType<HwanEmpireVigilant>().Count();

            IEnumerable<HwanEmpireVigilant> query1 = InteriorBuildings.OfType<HwanEmpireVigilant>();
            foreach (HwanEmpireVigilant Vigilant in query1)
            {
                Vigilant.Destroy();
            }

            base.OnDie(opposite);
        }

        protected override double CalculateAttackPower(double originalPower, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalPower + 5 * InteriorBuildings.OfType<HwanEmpireVigilant>().Count();
        }
    }

    public class HwanEmpireCityProductionFactory : ITileObjectProductionFactory
    {
        public static HwanEmpireCityProductionFactory Instance => _instance.Value;
        private static Lazy<HwanEmpireCityProductionFactory> _instance
            = new Lazy<HwanEmpireCityProductionFactory>(() => new HwanEmpireCityProductionFactory());

        private HwanEmpireCityProductionFactory()
        {
        }

        public Type ResultType => typeof(HwanEmpireCity);
        public ActorConstants Constants => HwanEmpireCity.Constants;

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

            return new HwanEmpireCity(owner, point, false);
        }
    }
}
