using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class JackieChan : Unit
    {
        private readonly IActorAction _holdingAttackAct;
        public override IActorAction HoldingAttackAct => _holdingAttackAct;

        private readonly IActorAction _movingAttackAct;
        public override IActorAction MovingAttackAct => _movingAttackAct;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];


        public JackieChan(Player owner, Terrain.Point point) : base(owner, typeof(JackieChan), point)
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

            public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
            {
                if (CheckError(origin, target) != null)
                    return double.NaN;

                return 1;
            }

            private Exception CheckError(Terrain.Point origin, Terrain.Point? target)
            {
                if (target != null)
                    return new ArgumentException("target is invalid");
                if (Owner.Owner.Game.TurnNumber <= LastSkillCalled + 4)
                    return new InvalidOperationException("Skill is not turned on");

                return null;
            }

            public void Act(Terrain.Point? target)
            {
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");
                var origin = _owner.PlacedPoint.Value;

                if (CheckError(origin, target) is Exception e)
                    throw e;

                ActionPoint Ap = GetRequiredAP(origin, target);
                if (!Owner.CanConsumeAP(Ap))
                    throw new InvalidOperationException("Not enough Ap");

                int A = origin.Position.A;
                int B = origin.Position.B;
                int C = origin.Position.C;
                var terrain = origin.Terrain;

                if (B + (C - 1 + Math.Sign(C - 1)) / 2 >= 0 && C - 1 >= 0)
                {
                    if ((terrain.GetPoint(A + 1, B, C - 1)).Unit != null)
                    {
                        if ((terrain.GetPoint(A + 1, B, C - 1)).Unit.Owner != Owner.Owner && (terrain.GetPoint(A + 1, B, C - 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (terrain.GetPoint(A + 1, B, C - 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (terrain.GetPoint(A + 1, B, C - 1)).Unit, 0, false, true);
                        }
                    }
                }

                if (B - 1 + (C + Math.Sign(C)) / 2 >= 0)
                {
                    if ((terrain.GetPoint(A + 1, B - 1, C)).Unit != null)
                    {
                        if ((terrain.GetPoint(A + 1, B - 1, C)).Unit.Owner != Owner.Owner && (terrain.GetPoint(A + 1, B - 1, C)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (terrain.GetPoint(A + 1, B - 1, C)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (terrain.GetPoint(A + 1, B - 1, C)).Unit, 0, false, true);
                        }
                    }
                }

                if (B + 1 + (C - 1 + Math.Sign(C - 1)) / 2 < terrain.Width && C - 1 >= 0)
                {
                    if ((terrain.GetPoint(A, B + 1, C - 1)).Unit != null)
                    {
                        if ((terrain.GetPoint(A, B + 1, C - 1)).Unit.Owner != Owner.Owner && (terrain.GetPoint(A, B + 1, C - 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (terrain.GetPoint(A, B + 1, C - 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (terrain.GetPoint(A, B + 1, C - 1)).Unit, 0, false, true);
                        }
                    }
                }

                if (B + 1 + (C + Math.Sign(C)) / 2 < terrain.Width)
                {
                    if ((terrain.GetPoint(A - 1, B + 1, C)).Unit != null)
                    {
                        if ((terrain.GetPoint(A - 1, B + 1, C)).Unit.Owner != Owner.Owner && (terrain.GetPoint(A - 1, B + 1, C)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (terrain.GetPoint(A - 1, B + 1, C)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (terrain.GetPoint(A - 1, B + 1, C)).Unit, 0, false, true);
                        }
                    }
                }

                if (B - 1 + (C + 1 + Math.Sign(C + 1)) / 2 >= 0 && C + 1 < terrain.Height)
                {
                    if ((terrain.GetPoint(A, B - 1, C + 1)).Unit != null)
                    {
                        if ((terrain.GetPoint(A, B - 1, C + 1)).Unit.Owner != Owner.Owner && (terrain.GetPoint(A, B - 1, C + 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (terrain.GetPoint(A, B - 1, C + 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (terrain.GetPoint(A, B - 1, C + 1)).Unit, 0, false, true);
                        }
                    }
                }

                if (B + (C + 1 + Math.Sign(C + 1)) / 2 < terrain.Width && C + 1 < terrain.Height)
                {
                    if ((terrain.GetPoint(A - 1, B, C + 1)).Unit != null)
                    {
                        if ((terrain.GetPoint(A - 1, B, C + 1)).Unit.Owner != Owner.Owner && (terrain.GetPoint(A - 1, B, C + 1)).Unit.BattleClassLevel < 4)
                        {
                            double Damage = (terrain.GetPoint(A - 1, B, C + 1)).Unit.MaxHP;
                            Owner.AttackTo(Damage, (terrain.GetPoint(A - 1, B, C + 1)).Unit, 0, false, true);
                        }
                    }
                }

                LastSkillCalled = Owner.Owner.Game.TurnNumber;
                Owner.ConsumeAP(Ap);
            }

        }
    }


    public class JackieChanProductionFactory : IActorProductionFactory
    {
        private static Lazy<JackieChanProductionFactory> _instance
            = new Lazy<JackieChanProductionFactory>(() => new JackieChanProductionFactory());
        public static JackieChanProductionFactory Instance => _instance.Value;
        private JackieChanProductionFactory()
        {
        }

        public Type ResultType => typeof(JackieChan);

        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityBase
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner, Terrain.Point point)
        {
            return new JackieChan(owner,point);
        }
    }
}
