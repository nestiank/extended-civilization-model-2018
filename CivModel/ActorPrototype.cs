using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="Actor"/>.
    /// </summary>
    /// <seealso cref="ProductionResultPrototype"/>
    /// <seealso cref="Actor"/>
    public class ActorPrototype : ProductionResultPrototype
    {
        /// <summary>
        /// The maximum AP.
        /// </summary>
        public double MaxAP { get; }

        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        public double MaxHP { get; }

        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        public double MaxHealPerTurn { get; }

        /// <summary>
        /// The attack power.
        /// </summary>
        public double AttackPower { get; }

        /// <summary>
        /// The defence power.
        /// </summary>
        public double DefencePower { get; }

        /// <summary>
        /// The amount of gold logistics per turn of this actor.
        /// Actor is starved if the owner cannot pay this logistics.
        /// </summary>
        public double GoldLogistics { get; }

        /// <summary>
        /// The amount of labor logistics per turn of this actor.
        /// Actor is starved if the owner cannot pay this logistics.
        /// </summary>
        public double LaborLogistics { get; }

        /// <summary>
        /// The amount of labor for this actor to get the full heal amount of <see cref="MaxHealPerTurn"/>.
        /// </summary>
        public double FullLaborForRepair { get; }

        /// <summary>
        /// Battle class level of this actor. This value can affect the ATK/DEF power during battle.
        /// </summary>
        public int BattleClassLevel { get; }

        /// <summary>
        /// The information about passive skills.
        /// </summary>
        public IReadOnlyList<SkillInfo> PassiveSkills { get; }

        /// <summary>
        /// The information about active skills.
        /// </summary>
        public IReadOnlyList<SkillInfo> ActiveSkills { get; }

        internal ActorPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            MaxAP = Convert.ToDouble(node.Element(xmlns + "MaxAP").Value);
            MaxHP = Convert.ToDouble(node.Element(xmlns + "MaxHP").Value);
            MaxHealPerTurn = Convert.ToDouble(node.Element(xmlns + "MaxHealPerTurn").Value);
            AttackPower = Convert.ToDouble(node.Element(xmlns + "AttackPower").Value);
            DefencePower = Convert.ToDouble(node.Element(xmlns + "DefencePower").Value);
            GoldLogistics = Convert.ToDouble(node.Element(xmlns + "GoldLogistics").Value);
            LaborLogistics = Convert.ToDouble(node.Element(xmlns + "LaborLogistics").Value);
            FullLaborForRepair = Convert.ToDouble(node.Element(xmlns + "FullLaborForRepair").Value);
            BattleClassLevel = Convert.ToInt32(node.Element(xmlns + "BattleClassLevel").Value);

            PassiveSkills = node.Elements(xmlns + "PassiveSkill")
                .Select(x => new SkillInfo(x)).ToArray();

            ActiveSkills = node.Elements(xmlns + "ActiveSkill")
                .Select(x => new SkillInfo(x)).ToArray();
        }
    }
}
