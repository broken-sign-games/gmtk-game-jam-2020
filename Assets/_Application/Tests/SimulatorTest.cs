using System.Collections;
using System.Collections.Generic;
using GMTK2020;
using GMTK2020.Data;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SimulatorTest
    {

        [Test]
        public void TestMoveTilesDown()
        {
            var intGrid = new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 3, 0, 0, 6 },
                { 0, 0, 0, 0, 0, 3, 3, 0, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 0, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 2, 3, 4 },
                { 0, 4, 4, 0, 0, 6, 9, 9, 5 },
                { 0, 0, 0, 0, 0, 7, 8, 9, 6 },
                { 0, 5, 5, 5, 0, 0, 7, 8, 7 },
            };
            Tile[,] grid = IntToTileGrid(intGrid);

            var pattern = new HashSet<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0)
            };
            var simulator = new Simulator(pattern);

            List<(Tile, Vector2Int)> movedTiles = simulator.MoveTilesDown(grid);

            var expected = IntToTileGrid(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 6 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 6 },
                { 0, 0, 0, 0, 0, 3, 3, 0, 6 },
                { 0, 0, 0, 0, 0, 3, 2, 3, 4 },
                { 0, 1, 0, 2, 0, 1, 9, 9, 5 },
                { 0, 4, 4, 2, 0, 6, 8, 9, 6 },
                { 0, 5, 5, 5, 0, 7, 7, 8, 7 },
            });

            for (int x = 0; x < expected.GetLength(0); ++x)
                for (int y = 0; y < expected.GetLength(1); ++y)
                {
                    Assert.That(grid[x, y]?.Color ?? 0, Is.EqualTo(expected[x, y]?.Color ?? 0));
                }
            Assert.That(movedTiles.Count, Is.EqualTo(14));
        }

        [Test]
        public void TestMatchRemovalWithTwoTilePattern()
        {
            var intGrid = new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 3, 0, 0, 6 },
                { 0, 0, 0, 0, 0, 3, 3, 0, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 0, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 2, 3, 4 },
                { 0, 4, 4, 0, 0, 6, 9, 9, 5 },
                { 0, 0, 0, 0, 0, 7, 8, 9, 6 },
                { 0, 5, 5, 5, 0, 0, 7, 8, 7 },
            };
            Tile[,] grid = IntToTileGrid(intGrid);

            var pattern = new HashSet<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0)
            };
            var simulator = new Simulator(pattern);

            HashSet<(Tile, Vector2Int)> removedTiles = simulator.RemoveMatchedTiles(grid);

            var expected = IntToTileGrid(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 2, 3, 4 },
                { 0, 0, 0, 0, 0, 6, 0, 0, 5 },
                { 0, 0, 0, 0, 0, 7, 8, 0, 6 },
                { 0, 0, 0, 0, 0, 0, 7, 8, 7 },
            });

            for (int x = 0; x < expected.GetLength(0); ++x)
                for (int y = 0; y < expected.GetLength(1); ++y)
                {
                    Assert.That(grid[x, y]?.Color ?? 0, Is.EqualTo(expected[x, y]?.Color ?? 0));
                }

            Assert.That(removedTiles.Count, Is.EqualTo(16));
        }
        [Test]
        public void TestMatchRemovalWithTetrominoPattern()
        {
            var intGrid = new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 3, 0, 0, 6 },
                { 0, 0, 0, 0, 0, 3, 3, 6, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 0, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 3, 4 },
                { 0, 0, 0, 0, 0, 1, 2, 9, 9 },
                { 0, 4, 4, 0, 0, 6, 9, 9, 5 },
                { 0, 0, 0, 5, 0, 7, 8, 9, 6 },
                { 0, 5, 5, 5, 0, 0, 7, 8, 7 },
            };
            Tile[,] grid = IntToTileGrid(intGrid);

            var pattern = new HashSet<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(2, 1)
            };
            var simulator = new Simulator(pattern);

            HashSet<(Tile, Vector2Int)> removedTiles = simulator.RemoveMatchedTiles(grid);

            var expected = IntToTileGrid(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 3, 0, 0, 6 },
                { 0, 0, 0, 0, 0, 3, 3, 6, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 0, 6 },
                { 0, 0, 0, 2, 0, 0, 0, 3, 4 },
                { 0, 0, 0, 0, 0, 1, 2, 0, 0 },
                { 0, 4, 4, 0, 0, 6, 0, 0, 5 },
                { 0, 0, 0, 0, 0, 7, 8, 0, 6 },
                { 0, 0, 0, 0, 0, 0, 7, 8, 7 },
            });

            for (int x = 0; x < expected.GetLength(0); ++x)
                for (int y = 0; y < expected.GetLength(1); ++y)
                {
                    Assert.That(grid[x, y]?.Color ?? 0, Is.EqualTo(expected[x, y]?.Color ?? 0));
                }

            Assert.That(removedTiles.Count, Is.EqualTo(9));
        }

        private static Tile[,] IntToTileGrid(int[,] intGrid)
        {
            int width = intGrid.GetLength(0);
            int height = intGrid.GetLength(1);
            var grid = new Tile[height, width];

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    int color = intGrid[x, y];
                    if (color == 0)
                        continue;

                    grid[y, width - x - 1] = new Tile(color);
                }

            return grid;
        }
    }
}
