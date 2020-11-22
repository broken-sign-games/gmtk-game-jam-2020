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

        [TestCase(0, 0, ExpectedResult = true)]
        [TestCase(3, 0, ExpectedResult = true)]
        [TestCase(0, 2, ExpectedResult = true)]
        [TestCase(3, 2, ExpectedResult = true)]
        [TestCase(1, 1, ExpectedResult = true)]
        [TestCase(4, 1, ExpectedResult = false)]
        [TestCase(-1, 1, ExpectedResult = false)]
        [TestCase(1, 3, ExpectedResult = false)]
        [TestCase(1, -1, ExpectedResult = false)]
        [TestCase(4, -1, ExpectedResult = false)]
        public bool Test_in_bounds_check_via_integers(int x, int y)
        {
            var board = new Board(4, 3);

            return board.IsInBounds(x, y);
        }

        [TestCase(0, 0, ExpectedResult = true)]
        [TestCase(3, 0, ExpectedResult = true)]
        [TestCase(0, 2, ExpectedResult = true)]
        [TestCase(3, 2, ExpectedResult = true)]
        [TestCase(1, 1, ExpectedResult = true)]
        [TestCase(4, 1, ExpectedResult = false)]
        [TestCase(-1, 1, ExpectedResult = false)]
        [TestCase(1, 3, ExpectedResult = false)]
        [TestCase(1, -1, ExpectedResult = false)]
        [TestCase(4, -1, ExpectedResult = false)]
        public bool Test_in_bounds_check_via_vector(int x, int y)
        {
            var board = new Board(4, 3);

            return board.IsInBounds(new Vector2Int(x, y));
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

        [TestCase(3, HorizontalOrder.LeftToRight, new[] { 0, 1, 2 })]
        [TestCase(3, HorizontalOrder.RightToLeft, new[] { 2, 1, 0 })]
        [TestCase(5, HorizontalOrder.LeftToRight, new[] { 0, 1, 2, 3, 4 })]
        [TestCase(5, HorizontalOrder.RightToLeft, new[] { 4, 3, 2, 1, 0 })]
        public void Get_x_indices(int width, HorizontalOrder order, int[] xs)
        {
            Board board = new Board(width, 4);

            Assert.That(board.GetXs(order), Is.EqualTo(xs));
        }

        [TestCase(3, VerticalOrder.BottomToTop, new[] { 0, 1, 2 })]
        [TestCase(3, VerticalOrder.TopToBottom, new[] { 2, 1, 0 })]
        [TestCase(5, VerticalOrder.BottomToTop, new[] { 0, 1, 2, 3, 4 })]
        [TestCase(5, VerticalOrder.TopToBottom, new[] { 4, 3, 2, 1, 0 })]
        public void Get_y_indices(int height, VerticalOrder order, int[] ys)
        {
            Board board = new Board(4, height);

            Assert.That(board.GetYs(order), Is.EqualTo(ys));
        }

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

        [TestCaseSource(nameof(allRowsTestCases))]
        public void Get_all_rows(HorizontalOrder horizontal, VerticalOrder vertical, int[][] colors)
        {
            Board board = GetTestBoard();

            Assert.That(board.GetRows(horizontal, vertical).Select(r => r.Select(t => t.Color)), Is.EqualTo(colors));
        }

        [TestCaseSource(nameof(allColumnsTestCases))]
        public void Get_all_columns(HorizontalOrder horizontal, VerticalOrder vertical, int[][] colors)
        {
            Board board = GetTestBoard();

            Assert.That(board.GetColumns(horizontal, vertical).Select(r => r.Select(t => t.Color)), Is.EqualTo(colors));
        }

        [Test]
        public void Move_tile()
        {
            var board = new Board(5, 5);
            var from = new Vector2Int(3, 4);
            var to = new Vector2Int(1, 2);
            var tile = new Tile(3);

            board[from] = tile;

            MovedTile movedTile = board.MoveTile(tile, to);

            Assert.That(board[from], Is.Null);
            Assert.That(board[to], Is.SameAs(tile));
            Assert.That(movedTile.Tile, Is.SameAs(tile));
            Assert.That(movedTile.From, Is.EqualTo(from));
            Assert.That(movedTile.To, Is.EqualTo(to));
        }

        [Test]
        public void Move_tile_via_integer_coordinates()
        {
            var board = new Board(5, 5);
            var from = new Vector2Int(3, 4);
            var to = new Vector2Int(1, 2);
            var tile = new Tile(3);

            board[from] = tile;

            MovedTile movedTile = board.MoveTile(tile, to.x, to.y);

            Assert.That(board[from], Is.Null);
            Assert.That(board[to], Is.SameAs(tile));
            Assert.That(movedTile.Tile, Is.SameAs(tile));
            Assert.That(movedTile.From, Is.EqualTo(from));
            Assert.That(movedTile.To, Is.EqualTo(to));
        }

        [Test]
        public void Cannot_move_null_tile()
        {
            var board = new Board(5, 5);
            Assert.Throws<NullReferenceException>(
                () => board.MoveTile(null, Vector2Int.zero));
        }

        [Test]
        public void Moving_tile_that_doesnt_exist_in_starting_position_doesnt_create_null()
        {
            var board = new Board(5, 5);
            var from = new Vector2Int(3, 4);
            var to = new Vector2Int(1, 2);
            var movingTile = new Tile(3);
            var otherTile = new Tile(4);

            board[from] = movingTile;
            board[from] = otherTile;

            board.MoveTile(movingTile, to);

            Assert.That(board[from], Is.SameAs(otherTile));
            Assert.That(board[to], Is.SameAs(movingTile));
        }

        [Test]
        public void Can_move_tile_from_off_the_board()
        {
            var board = new Board(5, 5);
            var from = new Vector2Int(3, 8);
            var to = new Vector2Int(1, 2);
            var tile = new Tile(3, from);

            MovedTile movedTile = board.MoveTile(tile, to);

            Assert.That(board[to], Is.SameAs(tile));
            Assert.That(movedTile.Tile, Is.SameAs(tile));
            Assert.That(movedTile.From, Is.EqualTo(from));
            Assert.That(movedTile.To, Is.EqualTo(to));
        }

        private static readonly object[] allRowsTestCases = {
            new object[] {
                HorizontalOrder.LeftToRight,
                VerticalOrder.BottomToTop,
                new[] {
                    new[] { 1, 2, 3 },
                    new[] { 4, 5, 6 },
                    new[] { 7, 8, 9 },
                }
            },
            new object[] {
                HorizontalOrder.LeftToRight,
                VerticalOrder.TopToBottom,
                new[] {
                    new[] { 7, 8, 9 },
                    new[] { 4, 5, 6 },
                    new[] { 1, 2, 3 },
                }
            },
            new object[] {
                HorizontalOrder.RightToLeft,
                VerticalOrder.BottomToTop,
                new[] {
                    new[] { 3, 2, 1 },
                    new[] { 6, 5, 4 },
                    new[] { 9, 8, 7 },
                }
            },
            new object[] {
                HorizontalOrder.RightToLeft,
                VerticalOrder.TopToBottom,
                new[] {
                    new[] { 9, 8, 7 },
                    new[] { 6, 5, 4 },
                    new[] { 3, 2, 1 },
                }
            }
        };

        private static readonly object[] allColumnsTestCases = {
            new object[] {
                HorizontalOrder.LeftToRight,
                VerticalOrder.BottomToTop,
                new[] {
                    new[] { 1, 4, 7 },
                    new[] { 2, 5, 8 },
                    new[] { 3, 6, 9 },
                }
            },
            new object[] {
                HorizontalOrder.LeftToRight,
                VerticalOrder.TopToBottom,
                new[] {
                    new[] { 7, 4, 1 },
                    new[] { 8, 5, 2 },
                    new[] { 9, 6, 3 },
                }
            },
            new object[] {
                HorizontalOrder.RightToLeft,
                VerticalOrder.BottomToTop,
                new[] {
                    new[] { 3, 6, 9 },
                    new[] { 2, 5, 8 },
                    new[] { 1, 4, 7 },
                }
            },
            new object[] {
                HorizontalOrder.RightToLeft,
                VerticalOrder.TopToBottom,
                new[] {
                    new[] { 9, 6, 3 },
                    new[] { 8, 5, 2 },
                    new[] { 7, 4, 1 },
                }
            }
        };

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
