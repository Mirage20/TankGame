using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TankGameXNAClient
{
    // Manages the game grid (bricks, stones, water and empty cells)
    class GridManager
    {
        private Cell[,] grid;
        private int Xc = Global.GridSizeX;  // columns
        private int Yc = Global.GridSizeY;  // rows
        public GridManager()
        {           
            grid = new Cell[Xc, Yc];
        }

        // build the initial grid
        public void BuildGrid()
        {
            for (int i = 0; i < Xc; i++)
            {
                for (int j = 0; j < Yc; j++)
                {
                    grid[i, j] = new CellEmpty();
                    grid[i, j].X = i;
                    grid[i, j].Y = j;
                }
            }
            
        }

        // setup the positions of bricks,stones and water cells
        public void SetStationaryGameObjects(Point[] brickPositions, Point[] stonePositions, Point[] waterPositions)
        {
            foreach(Point brickPosition in brickPositions)
            {
                CellBrick tmpCellBrick = new CellBrick();
                tmpCellBrick.X = brickPosition.X;
                tmpCellBrick.Y = brickPosition.Y;
                grid[brickPosition.X, brickPosition.Y] = tmpCellBrick;
            }

            foreach (Point stonePosition in stonePositions)
            {
                CellStone tmpCellStone = new CellStone();
                tmpCellStone.X = stonePosition.X;
                tmpCellStone.Y = stonePosition.Y;
                grid[stonePosition.X, stonePosition.Y] = tmpCellStone;
            }

            foreach (Point waterPosition in waterPositions)
            {
                CellWater tmpCellWater = new CellWater();
                tmpCellWater.X = waterPosition.X;
                tmpCellWater.Y = waterPosition.Y;
                grid[waterPosition.X, waterPosition.Y] = tmpCellWater;
            }
        }

        //return the entire grid
        public Cell[,] GetGrid()
        {
            return grid;
        }

        //update the brick damage
        public void UpdateBricks(int numberOfBricks, System.Drawing.Point[] brickPositions, BrickDamageLevel[] currentBrickDamageLevel)
        {
            for(int i=0;i<numberOfBricks;i++)
            {
                int tmpX=brickPositions[i].X;
                int tmpY=brickPositions[i].Y;
                if(grid[tmpX,tmpY] is CellBrick)
                {
                    ((CellBrick)(grid[tmpX, tmpY])).DamageLevel = currentBrickDamageLevel[i];
                }
            }
        }

        //draw  grid to the gui
        public void Draw()
        {
            for (int i = 0; i < Xc; i++)
            {
                for (int j = 0; j < Yc; j++)
                {
                    grid[i, j].Draw();                   
                }
            }
        }
    }
}
