using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
namespace TankGameXNAClient
{
    // Manages the AI in the game
    class AIManager
    {
        private Cell[,] grid;
        private List<Cell> openSet;
        private List<Cell> closeSet;
        private Dictionary<Cell, Cell> navigatedCells;
        public AIManager()
        {

        }
        // return next server command based on the current game data (Evaluation function)
        public string getNextCommand(GridManager gridManager, PlayerManager playerManager, ItemManager itemManager)
        {
            this.grid = gridManager.GetGrid();
            Point playerLocation = new Point(playerManager.OwnPlayer.X, playerManager.OwnPlayer.Y);
            Cell playerCurrentCell = gridManager.GetGrid()[playerLocation.X, playerLocation.Y];

            // Fire when enemy tank is in line of sight
            if (!Global.IsPeaceful && playerManager.OwnPlayer.CurrentHealth > Global.CriticalHealth)
            {
                foreach (Player player in playerManager.GetAllPlayers())
                {
                    if (player is PlayerAllied)
                        continue;
                    
                    Point enemyTarget = new Point(player.X, player.Y);
                    if (player.CurrentHealth > 0 && isCellInRegion(playerLocation, enemyTarget, Global.LineOfSight))
                    {
                        if (playerManager.OwnPlayer.PlayerDirection == Direction.North && enemyTarget.Y < playerLocation.Y && enemyTarget.X == playerLocation.X)
                            return "SHOOT#";
                        else if (playerManager.OwnPlayer.PlayerDirection == Direction.East && enemyTarget.X > playerLocation.X && enemyTarget.Y == playerLocation.Y)
                            return "SHOOT#";
                        else if (playerManager.OwnPlayer.PlayerDirection == Direction.South && enemyTarget.Y > playerLocation.Y && enemyTarget.X == playerLocation.X)
                            return "SHOOT#";
                        else if (playerManager.OwnPlayer.PlayerDirection == Direction.West && enemyTarget.X < playerLocation.X && enemyTarget.Y == playerLocation.Y)
                            return "SHOOT#";
                        else
                            continue;
                    }
                }

                // find for low health target
                if (Global.EnableKillSteal)
                {
                    foreach (Player player in playerManager.GetAllPlayers())
                    {
                        if (player is PlayerAllied)
                            continue;

                        Point enemyTarget = new Point(player.X, player.Y);
                        if (player.CurrentHealth > 0 && player.CurrentHealth <= Global.KillStealHealth && isCellInRegion(playerLocation, enemyTarget, Global.KillStealRange))
                        {
                            List<Cell> optimumPath = findPath(playerCurrentCell, gridManager.GetGrid()[enemyTarget.X, enemyTarget.Y]);
                            Point nextneighborCellLocation = new Point(optimumPath[optimumPath.Count - 1].X, optimumPath[optimumPath.Count - 1].Y);
                            return getMovementMessage(playerLocation, nextneighborCellLocation);
                        }
                    }
                }
            }

            // find and take coin piles and life packs when possible
            if (itemManager.GetCoinPiles().Count > 0 || itemManager.GetLifePacks().Count > 0)
            {
                for (int region = 0; region < Global.ScanRange; region++)
                {
                    if (playerManager.OwnPlayer.CurrentHealth > Global.CriticalHealth)
                    {
                        for (int i = 0; i < itemManager.GetCoinPiles().Count; i++)
                        {
                            Point tmpCoinLocation = new Point(itemManager.GetCoinPiles()[i].X, itemManager.GetCoinPiles()[i].Y);
                            if (isCellInRegion(playerLocation, tmpCoinLocation, region))
                            {
                                List<Cell> optimumPath = findPath(playerCurrentCell, gridManager.GetGrid()[tmpCoinLocation.X, tmpCoinLocation.Y]);
                                Point nextneighborCellLocation = new Point(optimumPath[optimumPath.Count - 1].X, optimumPath[optimumPath.Count - 1].Y);
                                return getMovementMessage(playerLocation, nextneighborCellLocation);
                            }
                        }
                    }

                    if (isTopPlace(playerManager) || playerManager.OwnPlayer.CurrentHealth < Global.MaxHealthTarget || itemManager.GetCoinPiles().Count == 0)
                    {
                        for (int i = 0; i < itemManager.GetLifePacks().Count; i++)
                        {
                            Point tmpLifeLocation = new Point(itemManager.GetLifePacks()[i].X, itemManager.GetLifePacks()[i].Y);
                            if (isCellInRegion(playerLocation, tmpLifeLocation, region))
                            {
                                List<Cell> optimumPath = findPath(playerCurrentCell, gridManager.GetGrid()[tmpLifeLocation.X, tmpLifeLocation.Y]);
                                Point nextneighborCellLocation = new Point(optimumPath[optimumPath.Count - 1].X, optimumPath[optimumPath.Count - 1].Y);
                                return getMovementMessage(playerLocation, nextneighborCellLocation);
                            }
                        }
                    }
                }

            }
            else
            {
                // search for enemy target and follow
                for (int region = 0; region < Global.ScanRange; region++)
                {
                    for (int i = 0; i < playerManager.GetAllPlayers().Length; i++)
                    {
                        if (playerManager.GetAllPlayers()[i] is PlayerAllied || playerManager.GetAllPlayers()[i].CurrentHealth <= 0)
                            continue;

                        Point tmpEnemyLocation = new Point(playerManager.GetAllPlayers()[i].X, playerManager.GetAllPlayers()[i].Y);
                        if (isCellInRegion(playerLocation, tmpEnemyLocation, region))
                        {
                            List<Cell> optimumPath = findPath(playerCurrentCell, gridManager.GetGrid()[tmpEnemyLocation.X, tmpEnemyLocation.Y]);
                            Point nextneighborCellLocation = new Point(optimumPath[optimumPath.Count - 1].X, optimumPath[optimumPath.Count - 1].Y);
                            return getMovementMessage(playerLocation, nextneighborCellLocation);
                        }
                    }
                }

            }

            return getMovementMessage(new Point(0, 0), new Point(0, 0));
        }

        // check if the source and target points within the given boundray
        private bool isCellInRegion(Point source, Point target, int regionScale)
        {
            if (Math.Abs(source.X - target.X) <= regionScale && Math.Abs(source.Y - target.Y) <= regionScale)
                return true;
            return false;
        }

        // check if the own player is in 1st place
        private bool isTopPlace(PlayerManager playerManager)
        {
            foreach (Player player in playerManager.GetAllPlayers())
            {
                if (playerManager.OwnPlayer.CurrentPoints < player.CurrentPoints)
                    return false;
            }
            return true;
        }

        // get the movement message based on the neighbor cell
        private string getMovementMessage(Point playerLocaation, Point neighborCellLocation)
        {
            if (playerLocaation.X > neighborCellLocation.X)
                return "LEFT#";
            if (playerLocaation.X < neighborCellLocation.X)
                return "RIGHT#";
            if (playerLocaation.Y > neighborCellLocation.Y)
                return "UP#";
            if (playerLocaation.Y < neighborCellLocation.Y)
                return "DOWN#";
            return "SHOOT#";
        }

        // A* path finding algorithm
        private List<Cell> findPath(Cell startCell, Cell endCell)
        {
            openSet = new List<Cell>();
            closeSet = new List<Cell>();
            navigatedCells = new Dictionary<Cell, Cell>();
            openSet.Add(startCell);
            startCell.FScore = startCell.GScore + getDistance(startCell, endCell);
            Cell currentCell;
            while (openSet.Count > 0)
            {
                currentCell = getLowFscoreCell();
                if (currentCell.X == endCell.X && currentCell.Y == endCell.Y)
                {
                    Debug.WriteLine("Goal reached");
                    return constructPath(navigatedCells, endCell);
                }

                openSet.Remove(currentCell);
                closeSet.Add(currentCell);

                List<Cell> neighborCells = getNeighbors(currentCell);

                foreach (Cell neighbor in neighborCells)
                {
                    if (closeSet.Contains(neighbor))
                        continue;

                    int gScore = currentCell.GScore + 1;

                    if (!openSet.Contains(neighbor) || gScore < neighbor.GScore)
                    {
                        neighbor.GScore = gScore;
                        neighbor.FScore = neighbor.GScore + getDistance(neighbor, endCell);
                        navigatedCells.Add(neighbor, currentCell);
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);

                    }
                }
            }

            return null;
        }

        // construct the optimum path
        private List<Cell> constructPath(Dictionary<Cell, Cell> navigatedCells, Cell endCell)
        {
            List<Cell> optimumPath = new List<Cell>();
            Cell current = endCell;
            while (navigatedCells.ContainsKey(current))
            {
                optimumPath.Add(current);
                current = navigatedCells[current];
            }
            return optimumPath;
        }
        // return the distance bettwenn 2 cells 
        private int getDistance(Cell neighbor, Cell endCell)
        {
            int XDistance = Math.Abs(endCell.X - neighbor.X);
            int YDistance = Math.Abs(endCell.Y - neighbor.Y);
            return XDistance + YDistance;
        }

        // get the near neighbour cells
        private List<Cell> getNeighbors(Cell currentCell)
        {
            int cellX = currentCell.X;
            int cellY = currentCell.Y;
            int cellSizeX = grid.GetLength(0) - 1;
            int cellSizeY = grid.GetLength(1) - 1;
            List<Cell> neighborCells = new List<Cell>();
            //For x
            if (cellX > 0 && cellX < cellSizeX)
            {
                neighborCells.Add(grid[cellX - 1, cellY]);
                neighborCells.Add(grid[cellX + 1, cellY]);
            }
            else if (cellX == 0)
            {
                neighborCells.Add(grid[cellX + 1, cellY]);
            }
            else if (cellX == cellSizeX)
            {
                neighborCells.Add(grid[cellX - 1, cellY]);
            }

            //For y
            if (cellY > 0 && cellY < cellSizeY)
            {
                neighborCells.Add(grid[cellX, cellY + 1]);
                neighborCells.Add(grid[cellX, cellY - 1]);
            }
            else if (cellY == 0)
            {
                neighborCells.Add(grid[cellX, cellY + 1]);
            }
            else if (cellY == cellSizeY)
            {
                neighborCells.Add(grid[cellX, cellY - 1]);
            }

            //remove blockers
            for (int i = 0; i < neighborCells.Count; i++)
            {
                if (neighborCells[i].IsBlockedMove)
                    neighborCells.Remove(neighborCells[i--]);

            }

            return neighborCells;
        }

        // returns the lowest F score cell in the open set
        private Cell getLowFscoreCell()
        {
            Cell lowestFCell = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (lowestFCell.FScore > openSet[i].FScore)
                {
                    lowestFCell = openSet[i];
                }
            }
            return lowestFCell;
        }
    }
}
