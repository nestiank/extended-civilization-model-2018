using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public interface IActorAction
    {
        string Name { get; }
        bool IsParametered { get; }

        int GetRequiredAction(Terrain.Point? pt);
        bool IsValidTarget(Terrain.Point? pt);

        void Act(Terrain.Point? pt);
    }

    public interface IActor
    {
        int MaxAction { get; }
        int RemainAction { get; }
        bool SkipFlag { get; set; }

        IReadOnlyList<IActorAction> ActionList { get; }

        void PreTurn();
        void PostTurn();
    }

    public interface ICombatActor : IActor
    {
        int MaxHP { get; }
        int HP { get; }

        void Attack(ICombatActor target);
    }
}
