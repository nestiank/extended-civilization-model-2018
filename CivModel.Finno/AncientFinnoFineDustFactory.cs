using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public sealed class AncientFinnoFineDustFactory : TileBuilding
    {
        public static Guid ClassGuid { get; } = new Guid("26F24220-2B77-4E81-A985-77F3BBC77832");
        public override Guid Guid => ClassGuid;

        public static readonly ActorConstants Constants = new ActorConstants
        {
            MaxHP = 20,
            DefencePower = 2,
            GoldLogistics = 20,
            LaborLogistics = 10,
            MaxHealPerTurn = 4
        };
        public override double ProvidedLabor => 20;

        private List<HappinessEffect> _effects = new List<HappinessEffect>();

        public AncientFinnoFineDustFactory(Player owner, Terrain.Point point, Player donator = null)
            : base(owner, Constants, point, donator)
        {
            CreateEffect();
        }

        protected override void OnAfterChangeOwner(Player prevOwner)
        {
            base.OnAfterChangeOwner(prevOwner);

            ClearEffect();
            CreateEffect();
        }

        protected override void OnBeforeDestroy()
        {
            ClearEffect();

            base.OnBeforeDestroy();
        }

        private void CreateEffect()
        {
            foreach (var player in Owner.Game.Players)
            {
                if (player.Team != Owner.Team)
                {
                    var effect = new HappinessEffect(player);
                    _effects.Add(effect);
                    effect.EffectOn();
                }
            }
        }

        private void ClearEffect()
        {
            foreach (var effect in _effects)
            {
                if (effect.Enabled)
                    effect.EffectOff();
            }
            _effects.Clear();
        }

        private class HappinessEffect : PlayerEffect
        {
            public HappinessEffect(Player player)
                : base(player, -1)
            {
            }

            protected override void OnEffectOn() { }
            protected override void OnEffectOff() { }
            protected override void OnTargetDestroy() { }

            protected override void FixedPostTurn()
            {
                Target.Happiness = Math.Max(-100, Target.Happiness - 5);
            }
        }
    }

    public class AncientFinnoFineDustFactoryProductionFactory : ITileBuildingProductionFactory
    {
        public static AncientFinnoFineDustFactoryProductionFactory Instance => _instance.Value;
        private static Lazy<AncientFinnoFineDustFactoryProductionFactory> _instance
            = new Lazy<AncientFinnoFineDustFactoryProductionFactory>(() => new AncientFinnoFineDustFactoryProductionFactory());
        private AncientFinnoFineDustFactoryProductionFactory()
        {
        }

        public Type ResultType => typeof(AncientFinnoFineDustFactory);
        public ActorConstants ActorConstants => AncientFinnoFineDustFactory.Constants;

        public double TotalLaborCost => 20;
        public double LaborCapacityPerTurn => 10;
        public double TotalGoldCost => 20;
        public double GoldCapacityPerTurn => 10;

        public Production Create(Player owner)
        {
            return new TileBuildingProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.TileBuilding == null && production.Owner.IsAlliedWith(point.TileOwner);
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new AncientFinnoFineDustFactory(owner, point);
        }
        public TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator)
        {
            return new AncientFinnoFineDustFactory(owner, point, donator);
        }
    }
}
