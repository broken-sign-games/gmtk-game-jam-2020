using GMTK2020.Data;
using NUnit.Framework;
using System;

namespace Tests
{
    public class BoardTest
    {
        [TestCase(1, 1)]
        [TestCase(2, 3)]
        [TestCase(7, 5)]
        [TestCase(9, 9)]
        [TestCase(15, 12)]
        public void Create_board_of_given_size(int width, int height)
        {
            var board = new Board(width, height);

            Assert.That(board.Width, Is.EqualTo(width));
            Assert.That(board.Height, Is.EqualTo(height));
        }

        [TestCase(0, 0)]
        [TestCase(0, 3)]
        [TestCase(5, 0)]
        [TestCase(-3, 4)]
        [TestCase(9, -1)]
        [TestCase(-6, -7)]
        public void Cannot_create_board_with_nonpositive_dimension(int width, int height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Board(width, height));
        }

        [Test]
        public void New_board_is_empty()
        {
            int width = 5;
            int height = 3;
            var board = new Board(width, height);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    Assert.That(board[x, y], Is.Null);
        }

        [TestCase(-1, 3)]
        [TestCase(5, 2)]
        [TestCase(4, -1)]
        [TestCase(0, 4)]
        public void Cannot_access_tiles_out_of_bounds(int x, int y)
        {
            var board = new Board(5, 4);

            Assert.Throws<IndexOutOfRangeException>(
                () =>
                {
                    Tile tile = board[x, y];
                });
        }
    }
}
