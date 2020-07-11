using NUnit.Framework;
using GMTK2020;
using Random = System.Random;

namespace Tests
{
    public class LevelTest
    {
        [Test]
        public void SomeLevelIsGenerated()
        {
            var rand = new Random();
            var tiles = new Tile[9, 9];
            for (int x = 0; x < tiles.GetLength(0); ++x)
            {
                for (int y = 0; y < tiles.GetLength(1); ++y)
                {
                    tiles[x, y] = new Tile(rand.Next(0, 5).ToString());
                }
            }

            var level = new Level(tiles);

            level.PrintToConsole();
        }

    }
}
