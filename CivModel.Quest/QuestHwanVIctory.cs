using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanVictory : QuestUltimateBase, ITileObjectObserver
    {
        private const string Autism = "autism";
        private const string Cthulhu = "cthulhu";
        private const string Alien = "alien";
        private const string Airspace = "airspace";
        private const string Moai = "moai";
        private const string Wonder = "wonder";
        private const string Kimchi = "kimchi";
        private const string Unicorn = "unicorn";
        private const string Jackie = "jackie";

        protected override string DelayProgress => "delay";

        protected override List<KeyValuePair<string, ISpecialResource>> RequiredResources { get; }
            = new List<KeyValuePair<string, ISpecialResource>>();

        public QuestHwanVictory(Game game)
            : base(null, game.GetPlayerHwan(), typeof(QuestHwanVictory))
        {
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Autism, SpecialResourceAutismBeamReflex.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Cthulhu, SpecialResourceCthulhuProjectInfo.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Alien, SpecialResourceAlienCommunication.Instance));

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
                    RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Airspace, SpecialResourceAirspaceDomination.Instance));
                    Progresses[Airspace].Enabled = true;
                    break;
                case 1:
                    RequiredResources.Add(new KeyValuePair<string, ISpecialResource>(Moai, SpecialResourceMoaiForceField.Instance));
                    Progresses[Moai].Enabled = true;
                    break;
                case 2:
                    Progresses[Wonder].Enabled = true;
                    break;
                case 3:
                    Progresses[Kimchi].Enabled = true;
                    break;
                case 4:
                    Progresses[Unicorn].Enabled = true;
                    break;
                case 5:
                    Progresses[Jackie].Enabled = true;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override Ending GetVictoryEnding()
        {
            return new HwanUltimateVictory(Game);
        }

        protected override QuestUltimateBase GetEnemyUltimateQuest()
        {
            return Game.GetPlayerFinno().Quests.OfType<QuestFinnoVictory>().FirstOrDefault();
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
                Progresses[Wonder].SafeSetValue(Requestee.TileBuildings.OfType<Hwan.Preternaturality>().Count());
                Progresses[Kimchi].SafeSetValue(Requestee.TileBuildings.OfType<Hwan.HwanEmpireKimchiFactory>().Count());
                Progresses[Unicorn].SafeSetValue(Requestee.Units.OfType<Hwan.UnicornOrder>().Count());
                Progresses[Jackie].SafeSetValue(Requestee.Units.OfType<Hwan.JackieChan>().Count());
            }
        }
        public void TileObjectPlaced(TileObject obj) { }
    }
}
