using GMTK2020.Data;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class MovedTileTest
    {
        [Test]
        public void Constructor_passes_values_through()
        {
            int color = 3;
            Vector2Int from = new Vector2Int(1, 2);
            Vector2Int to = new Vector2Int(4, 5);
            Tile tile = new Tile(color, Vector2Int.zero);

            MovedTile movedTile = new MovedTile(tile, from, to);

            Assert.That(movedTile.Tile, Is.SameAs(tile));
            Assert.That(movedTile.From, Is.EqualTo(from));
            Assert.That(movedTile.To, Is.EqualTo(to));
        }

        [Test]
        public void Convenience_constructor_uses_tile_position_as_destination()
        {
            int color = 3;
            Vector2Int from = new Vector2Int(1, 2);
            Vector2Int to = new Vector2Int(4, 5);
            Tile tile = new Tile(color, to);

            MovedTile movedTile = new MovedTile(tile, from);

            Assert.That(movedTile.Tile, Is.SameAs(tile));
            Assert.That(movedTile.From, Is.EqualTo(from));
            Assert.That(movedTile.To, Is.EqualTo(to));
        }
    }
}
