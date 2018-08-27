using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.EasterPlayerNumber;

namespace CivModel.Quests
{
    public class QuestSubMoaiForceField : Quest, ITileObjectObserver
    {
        private const string BuildCount = "BuildCount";

        public QuestSubMoaiForceField(Game game)
           : base(game.GetPlayerEaster(), game.GetPlayerHwan(), typeof(QuestSubMoaiForceField))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Game.Random.Next(2) == 0)
                Deploy();
        }

        protected override void OnAccept()
        {
            Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.TileObjectObservable.RemoveObserver(this);

            Progresses[BuildCount].Value = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceMoaiForceField.Instance] = 1;

            foreach (Player player in Game.Players)
            {
                if (player.Team == Game.Players[0].Team)
                {
                    foreach (TileBuilding Tbuild in player.TileBuildings)
                    {
                        if (Tbuild is Hwan.HwanEmpireFIRFortress)
                        {
                            ((Hwan.HwanEmpireFIRFortress)Tbuild).IsForceFieldOn = true;
                        }
                        else if(Tbuild is Zap.FIRFortress)
                        {
                            ((Zap.FIRFortress)Tbuild).IsForceFieldOn = true;
                        }
                    }
                }
            }

            Cleanup();
        }

        public void TileObjectProduced(TileObject obj)
        {
            if (obj is CivModel.Hwan.HwanEmpireFIRFortress moai && moai.Owner == Requester && moai.Donator == Requestee)
            {
                Progresses[BuildCount].Value += 1;

                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
