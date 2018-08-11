using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class ActorPrototype : ProductionResultPrototype
    {
        public double MaxAP { get; }
        public double MaxHP { get; }
        public double MaxHealPerTurn { get; }
        public double AttackPower { get; }
        public double DefencePower { get; }
        public double GoldLogistics { get; }
        public double LaborLogistics { get; }
        public double FullLaborForRepair { get; }
        public int BattleClassLevel { get; }

        public ActorPrototype(XElement node, Assembly packageAssembly)
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
        }
    }
}
