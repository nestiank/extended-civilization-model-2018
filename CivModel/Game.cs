using System;
using System.Collections.Generic;
using System.Text;

namespace CivModel
{
    public class Game
    {
        private readonly Terrain _terrain;
        public Terrain Terrain => _terrain;

        public Game(int width, int height)
        {
            _terrain = new Terrain(width, height);
        }
    }
}
