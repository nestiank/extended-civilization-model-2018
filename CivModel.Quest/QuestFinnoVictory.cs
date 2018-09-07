using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoVictory : QuestUltimateBase, ITileObjectObserver
    {
        private const string Autism = "autism";
        private const string Necro = "necro";
        private const string Rlyeh = "rlyeh";
        private const string Stellar = "stellar";
        private const string Uber = "uber";
        private const string Wonder = "wonder";
        private const string Dust = "dust";
        private const string Eleph = "eleph";
        private const string Khan = "khan";

        protected override string DelayProgress => "delay";

        protected override List<KeyValuePair<string, ISpecialResource>> RequiredResources { get; }
            = new List<KeyValuePair<string, ISpecialResource>>();

        public QuestFinnoVictory(Game game)
            : base(null, game.GetPlayerFinno(), typeof(QuestFinnoVictory))
        {
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Autism, AutismBeamAmplificationCrystal.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Necro, Necronomicon.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Rlyeh, GatesOfRlyeh.Instance));

            int r1 = game.Random.Next(4);
            int r2 = game.Random.Next(r1 + 1, 5);
            int r3 = game.Random.Next(r2 + 1, 6);
            EnableRandom(r1);
            EnableRandom(r2);
            EnableRandom(r3);
        }

        private void EnableRandom(int index)
        {
            switch (index)
            {
                case 0:
                    RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Stellar, InterstellarEnergyExtractor.Instance));
                    Progresses[Stellar].Enabled = true;
                    break;
                case 1:
                    RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("uber", Ubermensch.Instance));
                    Progresses[Uber].Enabled = true;
                    break;
                case 2:
                    Progresses[Wonder].Enabled = true;
                    break;
                case 3:
                    Progresses[Dust].Enabled = true;
                    break;
                case 4:
                    Progresses[Eleph].Enabled = true;
                    break;
                case 5:
                    Progresses[Khan].Enabled = true;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override QuestUltimateBase GetEnemyUltimateQuest()
        {
            return Game.GetPlayerHwan().Quests.OfType<QuestHwanVictory>().FirstOrDefault();
        }

        protected override void OnAccept()
        {
            base.OnAccept();
            Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            Game.TileObjectObservable.RemoveObserver(this);
        }

        public void TileObjectProduced(TileObject obj)
        {
            if (obj is Actor actor && actor.Owner == Requestee)
            {
                Progresses[Wonder].SafeSetValue(Requestee.TileBuildings.OfType<Finno.Preternaturality>().Count());
                Progresses[Dust].SafeSetValue(Requestee.TileBuildings.OfType<Finno.AncientFinnoFineDustFactory>().Count());
                Progresses[Eleph].SafeSetValue(Requestee.Units.OfType<Finno.ElephantCavalry>().Count());
                Progresses[Khan].SafeSetValue(Requestee.Units.OfType<Finno.GenghisKhan>().Count());
            }
        }
        public void TileObjectPlaced(TileObject obj) { }
    }
}
