using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CivModel.Common;

namespace CivModel
{
    public class Game
    {
        public double GoldCoefficient => 1;
        public double PopulationCoefficient => 1;
        public double HappinessCoefficient => 1;
        public double LaborCoefficient => 0.1;
        public double LaborHappinessConstant => 0;

        private readonly Terrain _terrain;
        public Terrain Terrain => _terrain;

        private List<Player> _players = new List<Player>();
        public IReadOnlyList<Player> Players => _players;

        public int SubTurnNumber { get; private set; } = 0;
        public int TurnNumber => SubTurnNumber / Players.Count;

        public bool IsInsideTurn { get; private set; } = false;
        public int PlayerNumberInTurn => SubTurnNumber % Players.Count;
        public Player PlayerInTurn => Players[PlayerNumberInTurn];

        public Game(int width, int height, int numOfPlayer)
        {
            if (width <= 0)
                throw new ArgumentException("width must be positive");
            if (height <= 0)
                throw new ArgumentException("width must be positive");
            if (numOfPlayer <= 0)
                throw new ArgumentException("width must be positive");

            _terrain = new Terrain(width, height);

            for (int i = 0; i < numOfPlayer; ++i)
            {
                _players.Add(new Player(this));
            }

            var random = new Random();
            foreach (var player in Players)
            {
                Terrain.Point pt;
                do
                {
                    int x = random.Next((int)Math.Floor(Terrain.Width * 0.1),
                        (int)Math.Ceiling(Terrain.Width * 0.9));
                    int y = random.Next((int)Math.Floor(Terrain.Height * 0.1),
                        (int)Math.Ceiling(Terrain.Height * 0.9));

                    pt = Terrain.GetPoint(x, y);
                } while (pt.Unit != null);

                var pionner = new Pioneer(player);
                pionner.PlacedPoint = pt;
            }
        }

        public void StartTurn()
        {
            if (SubTurnNumber % Players.Count == 0)
            {
                foreach (Player p in Players)
                {
                    foreach (var unit in p.Units)
                        unit.PreTurn();
                    foreach (var city in p.Cities)
                        city.PreTurn();

                    p.PreTurn();
                }
            }

            foreach (Player p in Players)
            {
                foreach (var unit in p.Units)
                    unit.PrePlayerSubTurn(PlayerInTurn);
                foreach (var city in p.Cities)
                    city.PrePlayerSubTurn(PlayerInTurn);

                p.PrePlayerSubTurn(PlayerInTurn);
            }

            IsInsideTurn = true;
        }

        public void EndTurn()
        {
            foreach (Player p in Players)
            {
                p.PostPlayerSubTurn(PlayerInTurn);

                foreach (var unit in p.Units)
                    unit.PostPlayerSubTurn(PlayerInTurn);
                foreach (var city in p.Cities)
                    city.PostPlayerSubTurn(PlayerInTurn);
            }

            if ((SubTurnNumber + 1) % Players.Count == 0)
            {
                foreach (Player p in Players)
                {
                    p.PostTurn();

                    foreach (var unit in p.Units)
                        unit.PostTurn();
                    foreach (var city in p.Cities)
                        city.PostTurn();
                }
            }

            ++SubTurnNumber;
            IsInsideTurn = false;
        }
    }
}
