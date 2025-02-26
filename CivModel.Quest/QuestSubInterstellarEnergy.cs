using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Zap.EmuPlayerNumber;

namespace CivModel.Quests
{
    public class QuestSubInterstellarEnergy : Quest, ITileObjectObserver
    {
        private const string PreternaturalityCount = "PreternaturalityCount";

        private bool _finBuilt = false;
        private bool _emuBuilt = false;

        public QuestSubInterstellarEnergy(Game game)
            : base(game.GetPlayerEmu(), game.GetPlayerFinno(), typeof(QuestSubInterstellarEnergy))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Game.Random.Next(10) == 0)
                Deploy();
        }

        protected override void OnAccept()
        {
            Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.TileObjectObservable.RemoveObserver(this);

            Progresses[PreternaturalityCount].Value = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[InterstellarEnergyExtractor.Instance] = 1;

            Cleanup();
        }

        public void TileObjectProduced(TileObject obj)
        {
            if (obj is CivModel.Finno.Preternaturality extractor)
            {
                if (extractor.Owner == Requestee)
                {
                    if (!_finBuilt)
                        Progresses[PreternaturalityCount].Value += 1;
                    _finBuilt = true;
                }

                if (extractor.Owner == Requester && extractor.Donator == Requestee)
                {
                    if(!_emuBuilt)
                        Progresses[PreternaturalityCount].Value += 1;
                    _emuBuilt = true;
                }
            }

            if (_finBuilt && _emuBuilt)
                Status = QuestStatus.Completed;
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
