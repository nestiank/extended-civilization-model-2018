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
    public class ActorConstants
    {
        /// <summary>
        /// The maximum AP.
        /// </summary>
        public double MaxAP { get; set; } = 0;
        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        public double MaxHP { get; set; } = 0;
        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        /// <seealso cref="Actor.HealByLogistics(double)" />
        public double MaxHealPerTurn { get; set; } = 5;
        /// <summary>
        /// The attack power.
        /// </summary>
        public double AttackPower { get; set; } = 0;
        /// <summary>
        /// The defence power.
        /// </summary>
        public double DefencePower { get; set; } = 0;
        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        public double GoldLogistics { get; set; } = 0;
        /// <summary>
        /// The amount of labor logistics of this actor to get the full heal amount of <see cref="Actor.MaxHealPerTurn"/>.
        /// </summary>
        /// <seealso cref="Actor.HealByLogistics(double)" />
        public double FullLaborLogistics { get; set; } = 0;
        /// <summary>
        /// Battle class level of this actor. This value can affect the ATK/DEF power during battle.
        /// </summary>
        public int BattleClassLevel { get; set; } = 0;

        /// <summary>
        /// Create the copy of this object.
        /// </summary>
        /// <returns>The copy of this object.</returns>
        public ActorConstants Clone()
        {
            return (ActorConstants)MemberwiseClone();
        }
    }
}
