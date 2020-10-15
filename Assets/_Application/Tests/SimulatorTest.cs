using System;
using System.Collections.Generic;
using System.Linq;
using GMTK2020;
using GMTK2020.Data;
using NUnit.Framework;

namespace Tests
{
    public class SimulatorTest
    {
        [Test]
        public void Test_simulation_step_with_matches()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0, 0, 0 },
                { 0, 3,-1, 7, 1, 3, 4, 5, 6 },
                { 0, 2,-1, 8, 9,-2,-2,-2, 7 },
                { 4, 5,-1, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 0, 0, 0, 0, 0 },
                { 0, 3, 0, 7, 1, 0, 0, 0, 6 },
                { 0, 2, 0, 8, 9, 3, 4, 5, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(6));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(4));

            AssertThatBoardsAreEqual(board, expected);
        }

        [Test]
        public void Unmarked_tiles_dont_match()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0, 0, 0 },
                { 0, 3, 1, 7, 1, 3, 4, 5, 6 },
                { 0, 2,-1, 8, 9,-2,-2,-2, 7 },
                { 4, 5,-1, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0, 0, 0 },
                { 0, 3, 1, 7, 1, 0, 0, 0, 6 },
                { 0, 2,-1, 8, 9, 3, 4, 5, 7 },
                { 4, 5,-1, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(3));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(3));

            AssertThatBoardsAreEqual(board, expected);
        }

        [Test]
        public void Matches_can_be_longer_than_three()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0, 0, 0 },
                { 0, 3,-1, 7, 1, 3, 4, 5, 6 },
                { 0, 2,-1, 8,-2,-2,-2,-2,-2 },
                { 4, 5,-1, 6, 2, 3, 5, 4, 1 },
                { 1, 2,-1, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 0, 0, 0, 0, 0 },
                { 0, 3, 0, 7, 0, 0, 0, 0, 0 },
                { 0, 2, 0, 8, 1, 3, 4, 5, 6 },
                { 4, 5, 0, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 5, 2, 1, 4, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(9));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(6));

            AssertThatBoardsAreEqual(board, expected);
        }
        [Test]
        public void Matches_can_overlap()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0,-2, 0 },
                { 0, 3,-1, 7, 1, 3, 4,-2, 6 },
                { 0,-1,-1,-1, 9,-2,-2,-2, 7 },
                { 4, 5,-1, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 3, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(10));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(7));

            AssertThatBoardsAreEqual(board, expected);
        }

        [Test]
        public void Return_cleanup_step_when_no_matches_found()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 3, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 3, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<CleanUpStep>());

            CleanUpStep cleanUpStep = step as CleanUpStep;

            Assert.That(cleanUpStep.InertTiles.Count, Is.EqualTo(0));
            Assert.That(cleanUpStep.NewTiles.Count, Is.EqualTo(53));
            Assert.That(cleanUpStep.NewTiles.Select(mt => mt.Tile.Position).Distinct().Count(), Is.EqualTo(53));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            foreach (MovedTile newTile in cleanUpStep.NewTiles)
                Assert.That(expected[newTile.Tile.Position], Is.Null);
        }

        [Test]
        public void Remaining_marked_tiles_are_made_inert()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 3, 4, 0, 7 },
                { 4,-5,-5,-6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 3, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });
            expected[1, 1].MakeInert();
            expected[2, 1].MakeInert();
            expected[3, 1].MakeInert();

            var simulator = new Simulator(board, 9);

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<CleanUpStep>());

            CleanUpStep cleanUpStep = step as CleanUpStep;

            Assert.That(cleanUpStep.InertTiles.Count, Is.EqualTo(3));
            Assert.That(cleanUpStep.NewTiles.Count, Is.EqualTo(53));
            Assert.That(cleanUpStep.NewTiles.Select(mt => mt.Tile.Position).Distinct().Count(), Is.EqualTo(53));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            foreach (MovedTile newTile in cleanUpStep.NewTiles)
                Assert.That(expected[newTile.Tile.Position], Is.Null);
        }
        [Test]
        public void Simulate_to_stop()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0,-2, 0, 0 },
                { 0, 4, 5, 6, 0, 0,-3, 0, 0 },
                { 0, 3,-1, 7, 1, 3,-3, 5, 6 },
                { 0, 2,-1, 8, 9,-2,-2,-2, 7 },
                { 4, 5,-1, 6,-2,-2,-3, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 0, 0, 0, 0, 0 },
                { 0, 3, 0, 7, 0, 0, 0, 0, 6 },
                { 0, 2, 0, 8, 1, 0, 0, 5, 7 },
                { 4, 5, 5, 6, 9, 3, 0, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            List<SimulationStep> steps = simulator.SimulateToStop();

            Assert.That(steps.Count, Is.EqualTo(4));
            Assert.That(steps[0], Is.TypeOf<MatchStep>());
            Assert.That(steps[1], Is.TypeOf<MatchStep>());
            Assert.That(steps[2], Is.TypeOf<MatchStep>());
            Assert.That(steps[3], Is.TypeOf<CleanUpStep>());

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            var cleanUpStep = steps[3] as CleanUpStep;

            foreach (MovedTile newTile in cleanUpStep.NewTiles)
                Assert.That(expected[newTile.Tile.Position], Is.Null);
        }

        private static Board IntGridToBoard(int[,] intGrid)
        {
            int width = intGrid.GetLength(0);
            int height = intGrid.GetLength(1);
            var board = new Board(height, width);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    int color = intGrid[x, y];
                    if (color == 0)
                        continue;

                    var tile = new Tile(Math.Abs(color) - 1);
                    board[y, width - x - 1] = tile;

                    if (color < 0)
                        tile.Marked = true;
                }

            return board;
        }

        private void AssertThatBoardsAreEqual(Board actual, Board expected)
        {
            foreach (int x in actual.GetXs())
                foreach (int y in actual.GetYs())
                {
                    Assert.That(actual[x, y], Is.EqualTo(expected[x, y]));
                }
        }

        private void AssertThatBoardsAreEqualUpToNulls(Board actual, Board expected)
        {
            foreach (int x in actual.GetXs())
                foreach (int y in actual.GetYs())
                {
                    if (actual[x, y] is null || expected[x, y] is null)
                        continue;

                    Assert.That(actual[x, y], Is.EqualTo(expected[x, y]));
                }
        }
    }
}
