using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public abstract class District : TileObject
    {
        private readonly Player _owner;
        public Player Owner => _owner;

        public District(Player owner) : base(TileTag.District)
        {
            _owner = owner;
        }
    }
}
