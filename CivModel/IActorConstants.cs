using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a constants storage of <see cref="Actor"/>.
    /// </summary>
    /// <seealso cref="Actor"/>
    public interface IActorConstants
    {
        /// <summary>
        /// The maximum AP.
        /// </summary>
        double MaxAP { get; }
        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        double MaxHP { get; }
        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        /// <seealso cref="Actor.HealByLogistics(double)" />
        double MaxHealPerTurn { get; }
        /// <summary>
        /// The attack power.
        /// </summary>
        double AttackPower { get; }
        /// <summary>
        /// The defence power.
        /// </summary>
        double DefencePower { get; }
        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        double GoldLogistics { get; }
        /// <summary>
        /// The amount of labor logistics of this actor to get the full heal amount of <see cref="Actor.MaxHealPerTurn"/>.
        /// </summary>
        /// <seealso cref="Actor.HealByLogistics(double)" />
        double FullLaborLogicstics { get; }
    }

    /// <summary>
    /// Represents <see cref="IActorConstants"/> with some default values.
    /// </summary>
    /// <seealso cref="IActorConstants"/>
    /// <seealso cref="Actor"/>
    /// <seealso cref="Actor(Player, IActorConstants, Terrain.Point, TileTag)"/>
    public abstract class ActorConstants : IActorConstants
    {
        /// <summary>
        /// The maximum AP.
        /// </summary>
        public abstract double MaxAP { get; }
        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        public virtual double MaxHP => 0;
        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        /// <seealso cref="Actor.HealByLogistics(double)" />
        public virtual double MaxHealPerTurn => 5;
        /// <summary>
        /// The attack power.
        /// </summary>
        public virtual double AttackPower => 0;
        /// <summary>
        /// The defence power.
        /// </summary>
        public virtual double DefencePower => 0;
        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        public abstract double GoldLogistics { get; }
        /// <summary>
        /// The amount of labor logistics of this actor to get the full heal amount of <see cref="Actor.MaxHealPerTurn"/>.
        /// </summary>
        /// <seealso cref="Actor.HealByLogistics(double)" />
        public abstract double FullLaborLogicstics { get; }
    }
}
