using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    // Manages the players of the game
    class PlayerManager
    {
        private Player[] players;
        private List<TankShell> tankShells;
        public int OwnPlayerNumber { get; set; }
        public Player OwnPlayer { get; private set; }
        public PlayerManager()
        {
            tankShells = new List<TankShell>();
        }

        // Create the initial player list
        public void CreatePlayers(int numberOfPlayers, int[] playerNumbers, Point[] playerPositions, Direction[] playerDirections)
        {
            players = new Player[numberOfPlayers];

            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (playerNumbers[i] == OwnPlayerNumber)
                {
                    players[i] = new PlayerAllied();
                    players[i].PlayerNumber = playerNumbers[i];
                    players[i].X = playerPositions[i].X;
                    players[i].Y = playerPositions[i].Y;
                    players[i].PlayerDirection = playerDirections[i];
                    OwnPlayer = players[i];
                    continue;
                }
                players[i] = new PlayerEnemy();
                players[i].PlayerNumber = playerNumbers[i];
                players[i].X = playerPositions[i].X;
                players[i].Y = playerPositions[i].Y;
                players[i].PlayerDirection = playerDirections[i];

            }

        }


        // return all players
        public Player[] GetAllPlayers()
        {
            return players;
        }


        //returns the all tank shells fired by the players
        public List<TankShell> GetTankShells()
        {
            return tankShells;
        }

        //remove specific tank shell from the live tank shell list
        public void RemoveTankShell(TankShell tankShell)
        {
            tankShells.Remove(tankShell);
        }


        //update the player status
        public void UpdatePlayers(int numberOfPlayers, int[] playerNumbers, Point[] playerCurrentPositions, Direction[] playerCurrentDirections, bool[] playerIsShooted, int[] playerCurrentHealth, int[] playerCurrentCoins, int[] playerCurrentPoints)
        {

            for (int i = 0; i < playerNumbers.Length; i++)
            {
                //int playerNumber=playerNumbers[i];
                players[i].X = playerCurrentPositions[i].X;
                players[i].Y = playerCurrentPositions[i].Y;
                players[i].PlayerDirection = playerCurrentDirections[i];
                players[i].IsShooted = playerIsShooted[i];
                players[i].CurrentHealth = playerCurrentHealth[i];
                players[i].CurrentCoins = playerCurrentCoins[i];
                players[i].CurrentPoints = playerCurrentPoints[i];

                if(players[i].IsShooted)
                {
                    TankShell tmpShell = new TankShell();
                    tmpShell.ShellDirection = players[i].PlayerDirection;
                    tmpShell.X = players[i].X;
                    tmpShell.Y = players[i].Y;
                    tankShells.Add(tmpShell);
                }
            }

        }

        

        // draw the players and tank shells
        public void Draw()
        {
            if (players != null)
            {
                
                foreach (Player player in players)
                {
                    if (player != null && player.CurrentHealth > 0)
                        player.Draw();
                }

                foreach(TankShell tankShell in tankShells)
                {
                    tankShell.Draw();
                }
            }
        }
    }
}
