using GMTK2020.Data;
using NUnit.Framework;

namespace Tests
{
    public class TileTest
    {
        private static readonly int[] colors = new[] { -1, 0, 1, 3, 12 };

        [Test]
        public void Color_returns_value_from_constructor([ValueSource(nameof(colors))] int color)
        {
            Tile tile = new Tile(color);

            Assert.That(tile.Color, Is.EqualTo(color));
        }

        [Test]
        public void Test_marked_getter_and_setter()
        {
            Tile tile = new Tile(3);

            Assert.That(tile.Marked, Is.False);

            tile.Marked = true;

            Assert.That(tile.Marked, Is.True);

            tile.Marked = false;

            Assert.That(tile.Marked, Is.False);
        }

        [Test]
        public void Make_inert()
        {
            Tile tile = new Tile(3);
            tile.MakeInert();

            Assert.That(tile.Inert, Is.True);
        }

        [Test]
        public void Making_tile_inert_unmarks_it()
        {
            Tile tile = new Tile(3)
            {
                Marked = true
            };
            tile.MakeInert();

            Assert.That(tile.Marked, Is.False);

        }
    }
}
