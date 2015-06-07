using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    // Manages the game object collisions
    class CollisionManager
    {

        public CollisionManager()
        {

        }

        // low accuracy collision detections for life packs and coin piles
        public void CheckEventCollisions(GridManager gridManager, PlayerManager playerManager, ItemManager itemManager)
        {
            foreach (Player player in playerManager.GetAllPlayers())
            {
                if (player.CurrentHealth > 0)
                {
                    for (int i = 0; i < itemManager.GetCoinPiles().Count; i++)
                    {
                        if (player.X == itemManager.GetCoinPiles()[i].X && player.Y == itemManager.GetCoinPiles()[i].Y)
                            itemManager.RemoveCoinPile(itemManager.GetCoinPiles()[i]);
                    }

                    for (int i = 0; i < itemManager.GetLifePacks().Count; i++)
                    {
                        if (player.X == itemManager.GetLifePacks()[i].X && player.Y == itemManager.GetLifePacks()[i].Y)
                            itemManager.RemoveLifePack(itemManager.GetLifePacks()[i]);
                    }                  
                }

            }
        }

        // high accuracy collision detection for tank shells and players
        public void CheckCollisions(GridManager gridManager, PlayerManager playerManager, ItemManager itemManager)
        {

            // detect collision between players and tank shells
            foreach (Player player in playerManager.GetAllPlayers())
            {
                if (player.CurrentHealth > 0)
                {
                    //for (int i = 0; i < itemManager.GetCoinPiles().Count; i++)
                    //{
                    //    if (player.X == itemManager.GetCoinPiles()[i].X && player.Y == itemManager.GetCoinPiles()[i].Y)
                    //        itemManager.RemoveCoinPile(itemManager.GetCoinPiles()[i]);
                    //}

                    //for (int i = 0; i < itemManager.GetLifePacks().Count; i++)
                    //{
                    //    if (player.X == itemManager.GetLifePacks()[i].X && player.Y == itemManager.GetLifePacks()[i].Y)
                    //        itemManager.RemoveLifePack(itemManager.GetLifePacks()[i]);
                    //}

                    for (int i = 0; i < playerManager.GetTankShells().Count; i++)
                    {
                        if (playerManager.GetTankShells()[i].HasMoved && playerManager.GetTankShells()[i].X == player.X && playerManager.GetTankShells()[i].Y == player.Y)
                        {
                            playerManager.RemoveTankShell(playerManager.GetTankShells()[i--]);
                        }
                    }
                }

            }

            // detect collision between grid and tank shells
            for (int i = 0; i < gridManager.GetGrid().GetLength(0); i++)
            {
                for (int j = 0; j < gridManager.GetGrid().GetLength(1); j++)
                {
                    if (gridManager.GetGrid()[i, j] is CellEmpty || gridManager.GetGrid()[i, j] is CellWater)
                        continue;

                    if (gridManager.GetGrid()[i, j] is CellBrick && ((CellBrick)gridManager.GetGrid()[i, j]).DamageLevel == BrickDamageLevel.FullyDamage)
                        continue;

                    for (int k = 0; k < playerManager.GetTankShells().Count; k++)
                    {
                        if (playerManager.GetTankShells()[k].X == gridManager.GetGrid()[i, j].X && playerManager.GetTankShells()[k].Y == gridManager.GetGrid()[i, j].Y)
                        {
                            playerManager.RemoveTankShell(playerManager.GetTankShells()[k]);
                        }
                    }

                }

            }

            // detect collision between players and players
            if (Global.EnableOccupiedCellDetection)
            {
                for (int i = 0; i < gridManager.GetGrid().GetLength(0); i++)
                {
                    for (int j = 0; j < gridManager.GetGrid().GetLength(1); j++)
                    {
                        if (gridManager.GetGrid()[i, j] is CellEmpty)
                            gridManager.GetGrid()[i, j].IsBlockedMove = false;
                        else
                            continue;
                        foreach (Player player in playerManager.GetAllPlayers())
                        {
                            if (player.CurrentHealth > 0)
                            {
                                if (player.X == gridManager.GetGrid()[i, j].X && player.Y == gridManager.GetGrid()[i, j].Y)
                                    gridManager.GetGrid()[i, j].IsBlockedMove = true;
                            }
                        }
                    }
                }
            }

            // detect collision between tank shells and grid boundray
            for (int i = 0; i < playerManager.GetTankShells().Count; i++)
            {
                if (playerManager.GetTankShells()[i].X > Global.GridSizeX || playerManager.GetTankShells()[i].Y > Global.GridSizeX || playerManager.GetTankShells()[i].X < 0 || playerManager.GetTankShells()[i].Y < 0)
                {
                    playerManager.RemoveTankShell(playerManager.GetTankShells()[i]);
                }
            }


        }


    }
}
