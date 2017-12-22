using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CivModel
{
    public class GameFactory
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Player> Players { get; set; }
        
        public Game Create()
        {
            return new Game(Width, Height, Players);
        }
    }

    public class Game
    {
        private readonly Terrain _terrain;
        public Terrain Terrain => _terrain;

        private List<Player> _players = new List<Player>();
        public IReadOnlyList<Player> Players => _players;

        public int TurnNumber { get; private set; }
        public int PrettyTurnNumber => TurnNumber / Players.Count;

        public Player PlayerInTurn => Players[TurnNumber % Players.Count];

        public Game(int width, int height, IEnumerable<Player> players)
        {
            _terrain = new Terrain(width, height);
            _players.AddRange(players);

            TurnNumber = 0;
            StartTurn();
        }

        public void StartTurn()
        {
            foreach (Player p in Players)
            {
                foreach (Unit unit in p.Units)
                {
                    unit.PreTurn();
                }
                p.PreTurn();
            }
        }

        public void EndTurn()
        {
            foreach (Player p in Players)
            {
                p.PostTurn();
                foreach (Unit unit in p.Units)
                {
                    unit.PostTurn();
                }
            }

            ++TurnNumber;
        }
    }
}
