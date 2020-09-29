using GMTK2020.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;

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

        [TestCase(0, 0)]
        [TestCase(2, 3)]
        [TestCase(5, 1)]
        public void Test_setter_and_getter(int x, int y)
        {
            var board = new Board(6, 7);

            var tile = new Tile(3);

            board[x, y] = tile;

            Assert.That(board[x, y], Is.SameAs(tile));
        }

        [TestCase(0, 0)]
        [TestCase(2, 3)]
        [TestCase(5, 1)]
        public void Setter_updates_position_on_tile(int x, int y)
        {
            var board = new Board(6, 7);

            var tile = new Tile(3);

            board[x, y] = tile;

            Vector2Int expectedPosition = new Vector2Int(x, y);
            Assert.That(board[x, y].Position, Is.EqualTo(expectedPosition));
        }

        [TestCase(0, 0)]
        [TestCase(2, 3)]
        [TestCase(5, 1)]
        public void Test_vector_setter_and_getter(int x, int y)
        {
            var board = new Board(6, 7);

            var tile = new Tile(3);

            var pos = new Vector2Int(x, y);
            board[pos] = tile;

            Assert.That(board[pos], Is.SameAs(tile));
        }

        [TestCase(0, 0)]
        [TestCase(2, 3)]
        [TestCase(5, 1)]
        public void Vector_setter_updates_position_on_tile(int x, int y)
        {
            var board = new Board(6, 7);

            var tile = new Tile(3);

            var pos = new Vector2Int(x, y);
            board[pos] = tile;

            Assert.That(board[pos].Position, Is.EqualTo(pos));
        }

        [Test]
        public void Iterating_over_board_gives_all_non_null_tiles()
        {
            var board = new Board(3, 3);

            var tiles = new List<Tile>
            {
                (board[0, 0] = new Tile(0)),
                (board[1, 0] = new Tile(1)),
                (board[1, 1] = new Tile(2)),
                (board[1, 2] = new Tile(3)),
                (board[2, 0] = new Tile(4)),
                (board[2, 1] = new Tile(5))
            };

            Assert.That(board, Is.EquivalentTo(tiles));
        }

        [TestCase(0, new[] { 1, 2, 3 })]
        [TestCase(1, new[] { 4, 5, 6 })]
        [TestCase(2, new[] { 7, 8, 9 })]
        public void Get_row_from_left_to_right(int y, int[] colors)
        {
            Board board = GetTestBoard();

            Assert.That(board.GetRow(y).Select(t => t.Color), Is.EqualTo(colors));
        }

        [TestCase(0, new[] { 3, 2, 1 })]
        [TestCase(1, new[] { 6, 5, 4 })]
        [TestCase(2, new[] { 9, 8, 7 })]
        public void Get_row_from_right_to_left(int y, int[] colors)
        {
            Board board = GetTestBoard();

            Assert.That(board.GetRow(y, HorizontalOrder.RightToLeft).Select(t => t.Color), Is.EqualTo(colors));
        }

        [TestCase(0, new[] { 1, 4, 7 })]
        [TestCase(1, new[] { 2, 5, 8 })]
        [TestCase(2, new[] { 3, 6, 9 })]
        public void Get_column_from_bottom_to_top(int y, int[] colors)
        {
            Board board = GetTestBoard();

            Assert.That(board.GetColumn(y).Select(t => t.Color), Is.EqualTo(colors));
        }

        [TestCase(0, new[] { 7, 4, 1 })]
        [TestCase(1, new[] { 8, 5, 2 })]
        [TestCase(2, new[] { 9, 6, 3 })]
        public void Get_column_from_top_to_bottom(int y, int[] colors)
        {
            Board board = GetTestBoard();

            Assert.That(board.GetColumn(y, VerticalOrder.TopToBottom).Select(t => t.Color), Is.EqualTo(colors));
        }

        private Board GetTestBoard()
        {
            var board = new Board(3, 3);

            board[0, 0] = new Tile(1);
            board[1, 0] = new Tile(2);
            board[2, 0] = new Tile(3);
            board[0, 1] = new Tile(4);
            board[1, 1] = new Tile(5);
            board[2, 1] = new Tile(6);
            board[0, 2] = new Tile(7);
            board[1, 2] = new Tile(8);
            board[2, 2] = new Tile(9);

            return board;
        }
    }
}
