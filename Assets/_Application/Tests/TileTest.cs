using GMTK2020.Data;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TileTest
    {
        private static readonly int[] colors = new[] { -1, 0, 1, 3, 12 };

        [Test]
        public void Color_returns_value_from_constructor([ValueSource(nameof(colors))] int color)
        {
            Tile tile = new Tile(color, Vector2Int.zero);

            Assert.That(tile.Color, Is.EqualTo(color));
        }

        [Test]
        public void Test_position_getter_and_setter()
        {
            Vector2Int initialPosition = new Vector2Int(4, 5);
            Tile tile = new Tile(3, initialPosition);

            Assert.That(tile.Position, Is.EqualTo(initialPosition));

            Vector2Int newPosition = new Vector2Int(3, 1);
            tile.Position = newPosition;

            Assert.That(tile.Position, Is.EqualTo(newPosition));
        }

        [Test]
        public void Test_marked_getter_and_setter()
        {
            Tile tile = new Tile(3, Vector2Int.zero);

            Assert.That(tile.Marked, Is.False);

            tile.Marked = true;

            Assert.That(tile.Marked, Is.True);

            tile.Marked = false;

            Assert.That(tile.Marked, Is.False);
        }

        [Test]
        public void Make_inert()
        {
            Tile tile = new Tile(3, Vector2Int.zero);
            tile.MakeInert();

            Assert.That(tile.Inert, Is.True);
        }

        [Test]
        public void Making_tile_inert_unmarks_it()
        {
            Tile tile = new Tile(3, Vector2Int.zero)
            {
                Marked = true
            };
            tile.MakeInert();

            Assert.That(tile.Marked, Is.False);

        }
    }
}
