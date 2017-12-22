using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public abstract class Unit : TileObject
    {
        private readonly Player _owner;
        public Player Owner => _owner;

        public int MaxAction => 2;
        public int RemainAction { get; private set; }

        public Unit(Player owner) : base(TileTag.Unit)
        {
            _owner = owner;
        }

        public void PreTurn()
        {
            RemainAction = MaxAction;
        }

        public void PostTurn()
        {

        }
    }
}
