using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class JackieChan : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("3FE5F3BA-29AC-4BFD-99D7-ABA7CE9F706A");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override double MaxHP => 50;

        public override double AttackPower => 35;
        public override double DefencePower => 10;

        public override int BattleClassLevel => 4;

        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];


        public JackieChan(Player owner, Terrain.Point point) : base(owner, point)
        {
            _holdingAttackAct = new AttackActorAction(this, false);
            _movingAttackAct = new AttackActorAction(this, true);
            _specialActs[0] = new JackieChanAction(this);
        }

        private class JackieChanAction : IActorAction
        {
            private readonly JackieChan _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

            public JackieChanAction(JackieChan owner)
            {
                _owner = owner;
            }

            public int LastSkillCalled = -5;

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 4)
                    return -1;
                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 4)
                    throw new InvalidOperationException("Skill is not turned on");

                int A = Owner.PlacedPoint.Value.Position.A;
                int B = Owner.PlacedPoint.Value.Position.B;
                int C = Owner.PlacedPoint.Value.Position.C;

                if (B + (C - 1 + Math.Sign(C - 1)) / 2 >= 0 && C - 1 >= 0)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B, C - 1)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B, C - 1)).Unit.Owner != Owner.Owner && (Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B, C - 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B, C - 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B, C - 1)).Unit, 0, false, true);
                        }
                    }
                }

                if (B - 1 + (C + Math.Sign(C)) / 2 >= 0)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B - 1, C)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B - 1, C)).Unit.Owner != Owner.Owner && (Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B - 1, C)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B - 1, C)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A + 1, B - 1, C)).Unit, 0, false, true);
                        }
                    }
                }

                if (B + 1 + (C - 1 + Math.Sign(C - 1)) / 2 < Owner.PlacedPoint.Value.Terrain.Width && C - 1 >= 0)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B + 1, C - 1)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B + 1, C - 1)).Unit.Owner != Owner.Owner && (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B + 1, C - 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B + 1, C - 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B + 1, C - 1)).Unit, 0, false, true);
                        }
                    }
                }

                if (B + 1 + (C + Math.Sign(C)) / 2 < Owner.PlacedPoint.Value.Terrain.Width)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B + 1, C)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B + 1, C)).Unit.Owner != Owner.Owner && (Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B + 1, C)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B + 1, C)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B + 1, C)).Unit, 0, false, true);
                        }
                    }
                }

                if (B - 1 + (C + 1 + Math.Sign(C + 1)) / 2 >= 0 && C + 1 < Owner.PlacedPoint.Value.Terrain.Height)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B - 1, C + 1)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A, B - 1, C + 1)).Unit.Owner != Owner.Owner && (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B - 1, C + 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B - 1, C + 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A, B - 1, C + 1)).Unit, 0, false, true);
                        }
                    }
                }

                if (B + (C + 1 + Math.Sign(C + 1)) / 2 < Owner.PlacedPoint.Value.Terrain.Width && C + 1 < Owner.PlacedPoint.Value.Terrain.Height)
                {
                    if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B, C + 1)).Unit != null)
                    {
                        if ((Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B, C + 1)).Unit.Owner != Owner.Owner && (Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B, C + 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B, C + 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (Owner.PlacedPoint.Value.Terrain.GetPoint(A - 1, B, C + 1)).Unit, 0, false, true);
                        }
                    }
                }

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
            }

        }
    }


    public class JackieChanProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<JackieChanProductionFactory> _instance
            = new Lazy<JackieChanProductionFactory>(() => new JackieChanProductionFactory());
        public static JackieChanProductionFactory Instance => _instance.Value;
        private JackieChanProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 100, 20, 50, 10);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new JackieChan(owner,point);
        }
    }
}
