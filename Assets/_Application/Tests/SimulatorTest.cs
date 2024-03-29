﻿using System;
using System.Collections.Generic;
using System.Linq;
using GMTK2020;
using GMTK2020.Data;
using NUnit.Framework;
using UnityEngine;

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

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(6));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(4));
            Assert.That(matchStep.LeftEndsOfHorizontalMatches, Is.EquivalentTo(new[] { new Vector2Int(5, 2) }));
            Assert.That(matchStep.BottomEndsOfVerticalMatches, Is.EquivalentTo(new[] { new Vector2Int(2, 1) }));

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

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(3));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(3));
            Assert.That(matchStep.LeftEndsOfHorizontalMatches, Is.EquivalentTo(new[] { new Vector2Int(5, 2) }));
            Assert.That(matchStep.BottomEndsOfVerticalMatches, Is.Empty);

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

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(9));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(6));
            Assert.That(matchStep.LeftEndsOfHorizontalMatches, Is.EquivalentTo(new[] { new Vector2Int(4, 2), new Vector2Int(5, 2), new Vector2Int(6, 2) }));
            Assert.That(matchStep.BottomEndsOfVerticalMatches, Is.EquivalentTo(new[] { new Vector2Int(2, 1), new Vector2Int(2, 0) }));

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

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(10));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(7));
            Assert.That(matchStep.LeftEndsOfHorizontalMatches, Is.EquivalentTo(new[] { new Vector2Int(1, 2), new Vector2Int(5, 2) }));
            Assert.That(matchStep.BottomEndsOfVerticalMatches, Is.EquivalentTo(new[] { new Vector2Int(2, 1), new Vector2Int(7, 2) }));

            AssertThatBoardsAreEqual(board, expected);
        }

        [Test]
        public void Wildcards_match_regardless_of_color()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0, 0, 0 },
                { 0, 3,-1, 7, 1, 3, 4, 5, 6 },
                { 0, 2,-1, 8, 9,-8,-2,-2, 7 },
                { 4, 5,-3, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });
            board[2, 1].MakeWildcard();
            board[5, 2].MakeWildcard();

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

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(6));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(4));
            Assert.That(matchStep.LeftEndsOfHorizontalMatches, Is.EquivalentTo(new[] { new Vector2Int(5, 2) }));
            Assert.That(matchStep.BottomEndsOfVerticalMatches, Is.EquivalentTo(new[] { new Vector2Int(2, 1) }));

            AssertThatBoardsAreEqual(board, expected);
        }

        [Test]
        public void Check_against_false_positives_when_wildcard_is_first()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0, 0, 0 },
                { 0, 3,-2, 7, 1, 3, 4, 5, 6 },
                { 0, 2,-1, 8, 9,-8,-2,-4, 7 },
                { 4, 5,-3, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });
            board[2, 1].MakeWildcard();
            board[5, 2].MakeWildcard();

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<CleanUpStep>());

            CleanUpStep cleanUpStep = step as CleanUpStep;

            Assert.That(cleanUpStep.InertTiles.Count, Is.EqualTo(6));
        }

        [Test]
        public void Wildcard_can_be_several_colors_in_overlap()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 5, 6, 0, 0, 0,-2, 0 },
                { 0, 3,-1, 7, 1, 3, 4,-2, 6 },
                { 0,-2,-3,-2, 9,-1,-1,-3, 7 },
                { 4, 5,-1, 6, 2, 3, 5,-4, 1 },
                { 1, 2, 3, 2, 1, 4, 2,-4, 6 },
            });
            board[2, 2].MakeWildcard();
            board[7, 2].MakeWildcard();

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 3, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 0, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 0, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            SimulationStep step = simulator.SimulateNextStep();

            Assert.That(step, Is.TypeOf<MatchStep>());

            MatchStep matchStep = step as MatchStep;

            Assert.That(matchStep.MatchedTiles.Count, Is.EqualTo(12));
            Assert.That(matchStep.MovedTiles.Count, Is.EqualTo(7));
            Assert.That(matchStep.LeftEndsOfHorizontalMatches, Is.EquivalentTo(new[] { new Vector2Int(1, 2), new Vector2Int(5, 2) }));
            Assert.That(matchStep.BottomEndsOfVerticalMatches, Is.EquivalentTo(new[] { new Vector2Int(2, 1), new Vector2Int(7, 0), new Vector2Int(7, 2) }));

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

            var simulator = new Simulator(board, GetLevelSpecification());

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
                { 4,-5,-5,-6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 4, 2, 5, 6 },
            });
            expected[1, 1].MakeInert();
            expected[2, 1].MakeInert();
            expected[3, 1].MakeInert();

            var simulator = new Simulator(board, GetLevelSpecification());

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

            var simulator = new Simulator(board, GetLevelSpecification());

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

        [Test]
        public void Check_for_possible_matches_when_none_exist()
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

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.False);
        }

        [Test]
        public void Check_for_possible_matches_when_blocked_by_inert_tiles()
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
                { 4, 5, 5, 5, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });
            board[1, 1].MakeInert();
            board[5, 1].MakeInert();

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.False);
        }

        [Test]
        public void Check_for_possible_matches_with_horizontal_match()
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
                { 4, 5, 5, 5, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 5, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.True);
        }

        [Test]
        public void Check_for_possible_matches_with_vertical_match()
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
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.True);
        }

        [Test]
        public void Check_for_possible_matches_with_wildcard()
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
                { 1, 2, 3, 2, 1, 5, 2, 5, 6 },
            });
            board[5, 0].MakeWildcard();

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.True);
        }

        [Test]
        public void Check_for_possible_matches_with_two_wildcards()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 1, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 5, 2, 5, 6 },
            });
            board[5, 0].MakeWildcard();
            board[5, 2].MakeWildcard();

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.True);
        }

        [Test]
        public void Check_for_possible_matches_with_three_wildcards()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 4, 0, 6, 1, 0, 0, 0, 6 },
                { 0, 3, 0, 7, 9, 1, 4, 0, 7 },
                { 4, 5, 5, 6, 2, 3, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 5, 2, 5, 6 },
            });
            board[5, 0].MakeWildcard();
            board[5, 1].MakeWildcard();
            board[5, 2].MakeWildcard();

            var simulator = new Simulator(board, GetLevelSpecification());

            Assert.That(simulator.FurtherMatchesPossible(), Is.True);
        }

        [Test]
        public void Test_remove_tool()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 0, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 9, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 1, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 2, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 3, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 7, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 1, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemoveTile(new Vector2Int(2, 1));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(1));
            Assert.That(step.RemovedTiles.First(), Is.EqualTo(new Tile(4, new Vector2Int(2, 1))));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(7));
            Assert.That(step.NewTiles.Count, Is.EqualTo(1));
            Assert.That(step.NewTiles[0].From, Is.EqualTo(new Vector2Int(2, 9)));
            Assert.That(step.NewTiles[0].To, Is.EqualTo(new Vector2Int(2, 8)));
        }

        [Test]
        public void Test_refill_tool()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4,-7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4,-7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Vector2Int pos = new Vector2Int(2, 3);
            board[pos].MakeInert();

            var simulator = new Simulator(board, GetLevelSpecification());

            RefillStep step = simulator.RefillTile(pos);

            Assert.That(step.AffectedTiles.Count, Is.EqualTo(1));
            Assert.That(step.AffectedTiles[0], Is.SameAs(board[pos]));

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(board[pos].Marked, Is.True);
            Assert.That(board[pos].Inert, Is.False);
        }

        [Test]
        public void Test_bomb_tool()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 0, 0, 0, 1, 1, 2, 3, 5 },
                { 2, 0, 0, 0, 4, 3, 5, 3, 4 },
                { 1, 0, 0, 0, 2, 4, 4, 4, 9 },
                { 1, 5, 9, 2, 5, 1, 3, 3, 3 },
                { 7, 6, 1, 6, 3, 1, 2, 2, 1 },
                { 8, 5, 2, 7, 1, 1, 2, 3, 6 },
                { 2, 3, 2, 2, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemoveBlock(new Vector2Int(2, 3));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(9));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(12));
            Assert.That(step.NewTiles.Count, Is.EqualTo(9));
        }

        [Test]
        public void Test_bomb_tool_at_boundary()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 9, 2, 1, 1, 2, 0, 0 },
                { 0, 0, 1, 6, 4, 3, 5, 0, 0 },
                { 1, 5, 2, 7, 2, 4, 4, 3, 5 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 4 },
                { 7, 1, 3, 4, 3, 1, 2, 4, 9 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 3 },
                { 2, 3, 1, 7, 9, 3, 4, 2, 1 },
                { 4, 5, 5, 6, 2, 2, 5, 3, 6 },
                { 1, 2, 3, 2, 1, 3, 2, 8, 7 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemoveBlock(new Vector2Int(0, 8));

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(4));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(0));
            Assert.That(step.NewTiles.Count, Is.EqualTo(4));

            step = simulator.RemoveBlock(new Vector2Int(8, 0));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(4));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(14));
            Assert.That(step.NewTiles.Count, Is.EqualTo(4));
        }

        [Test]
        public void Test_plus_bomb_tool()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 0, 0, 0, 1, 1, 2, 3, 5 },
                { 2, 5, 0, 2, 4, 3, 5, 3, 4 },
                { 1, 6, 0, 6, 2, 4, 4, 4, 9 },
                { 1, 5, 9, 7, 5, 1, 3, 3, 3 },
                { 7, 3, 1, 2, 3, 1, 2, 2, 1 },
                { 8, 1, 2, 4, 1, 1, 2, 3, 6 },
                { 2, 3, 2, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemovePlus(new Vector2Int(2, 3));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(5));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(14));
            Assert.That(step.NewTiles.Count, Is.EqualTo(5));
        }

        [Test]
        public void Test_plus_bomb_tool_at_boundary()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 9, 2, 1, 1, 2, 0, 0 },
                { 0, 6, 1, 6, 4, 3, 5, 3, 0 },
                { 1, 5, 2, 7, 2, 4, 4, 3, 5 },
                { 1, 3, 2, 2, 5, 1, 3, 4, 4 },
                { 7, 1, 3, 4, 3, 1, 2, 3, 9 },
                { 8, 4, 7, 6, 1, 1, 2, 2, 3 },
                { 2, 3, 1, 7, 9, 3, 4, 3, 1 },
                { 4, 5, 5, 6, 2, 2, 5, 8, 6 },
                { 1, 2, 3, 2, 1, 3, 2, 4, 7 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemovePlus(new Vector2Int(0, 8));

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(3));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(0));
            Assert.That(step.NewTiles.Count, Is.EqualTo(3));

            step = simulator.RemovePlus(new Vector2Int(8, 0));

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(3));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(15));
            Assert.That(step.NewTiles.Count, Is.EqualTo(3));
        }

        [Test]
        public void Test_clear_row_tool()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemoveRow(3);

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(9));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(45));
            Assert.That(step.NewTiles.Count, Is.EqualTo(9));
        }

        [Test]
        public void Test_clear_color()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 0, 0, 0, 2, 0, 0, 2, 3, 0 },
                { 0, 5, 0, 6, 0, 0, 5, 3, 0 },
                { 0, 6, 9, 7, 0, 0, 4, 4, 5 },
                { 0, 5, 2, 2, 4, 0, 3, 3, 4 },
                { 2, 3, 2, 4, 2, 3, 2, 2, 9 },
                { 7, 4, 3, 6, 5, 4, 2, 3, 3 },
                { 8, 3, 7, 7, 3, 3, 4, 8, 6 },
                { 2, 5, 5, 6, 9, 2, 5, 4, 7 },
                { 4, 2, 3, 2, 2, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RemovalStep step = simulator.RemoveColor(0);

            AssertThatBoardsAreEqualUpToNulls(board, expected);

            Assert.That(step.RemovedTiles.Count, Is.EqualTo(16));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(28));
            Assert.That(step.NewTiles.Count, Is.EqualTo(16));
        }

        [Test]
        public void Test_shuffle_board()
        {
            Board initialBoard = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board workingBoard = initialBoard.DeepCopy();

            var simulator = new Simulator(workingBoard, GetLevelSpecification());

            PermutationStep step = simulator.ShuffleBoard();

            AssertThatBoardsAreNotEqual(initialBoard, workingBoard);

            Assert.That(step.MovedTiles.Count, Is.EqualTo(81));
        }

        [Test]
        public void Test_clockwise_board_rotation()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 4, 2, 8, 7, 1, 1, 2, 1 },
                { 2, 5, 3, 4, 1, 3, 5, 6, 5 },
                { 3, 5, 1, 7, 3, 2, 2, 1, 9 },
                { 2, 6, 7, 6, 4, 2, 7, 6, 2 },
                { 1, 2, 9, 1, 3, 5, 2, 4, 1 },
                { 3, 2, 3, 1, 1, 1, 4, 3, 1 },
                { 2, 5, 4, 2, 2, 3, 4, 5, 2 },
                { 5, 4, 8, 3, 2, 3, 4, 3, 3 },
                { 6, 1, 7, 6, 1, 3, 9, 4, 5 }
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RotationStep step = simulator.RotateBoard(RotationSense.CW);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.Pivot, Is.EqualTo(new Vector2(4, 4)));
            Assert.That(step.RotationSense, Is.EqualTo(RotationSense.CW));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(80));
        }

        [Test]
        public void Test_counterclockwise_board_rotation()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 5, 4, 9, 3, 1, 6, 7, 1, 6 },
                { 3, 3, 4, 3, 2, 3, 8, 4, 5 },
                { 2, 5, 4, 3, 2, 2, 4, 5, 2 },
                { 1, 3, 4, 1, 1, 1, 3, 2, 3 },
                { 1, 4, 2, 5, 3, 1, 9, 2, 1 },
                { 2, 6, 7, 2, 4, 6, 7, 6, 2 },
                { 9, 1, 2, 2, 3, 7, 1, 5, 3 },
                { 5, 6, 5, 3, 1, 4, 3, 5, 2 },
                { 1, 2, 1, 1, 7, 8, 2, 4, 1 }
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RotationStep step = simulator.RotateBoard(RotationSense.CCW);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.Pivot, Is.EqualTo(new Vector2(4, 4)));
            Assert.That(step.RotationSense, Is.EqualTo(RotationSense.CCW));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(80));
        }

        [Test]
        public void Test_clockwise_2x2_block_rotation()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 6, 7, 3, 4, 8, 7 },
                { 4, 5, 5, 2, 9, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RotationStep step = simulator.Rotate2x2Block(new Vector2Int(3, 1), RotationSense.CW);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.Pivot, Is.EqualTo(new Vector2(3.5f, 1.5f)));
            Assert.That(step.RotationSense, Is.EqualTo(RotationSense.CW));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(4));
        }

        [Test]
        public void Test_counterclockwise_2x2_block_rotation()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 9, 2, 3, 4, 8, 7 },
                { 4, 5, 5, 7, 6, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RotationStep step = simulator.Rotate2x2Block(new Vector2Int(3, 1), RotationSense.CCW);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.Pivot, Is.EqualTo(new Vector2(3.5f, 1.5f)));
            Assert.That(step.RotationSense, Is.EqualTo(RotationSense.CCW));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(4));
        }

        [Test]
        public void Test_clockwise_3x3_block_rotation()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 3, 5, 1, 3, 4, 8, 7 },
                { 4, 5, 2, 6, 7, 2, 5, 4, 1 },
                { 1, 2, 1, 2, 9, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RotationStep step = simulator.Rotate3x3Block(new Vector2Int(3, 1), RotationSense.CW);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.Pivot, Is.EqualTo(new Vector2(3f, 1f)));
            Assert.That(step.RotationSense, Is.EqualTo(RotationSense.CW));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(8));
        }

        [Test]
        public void Test_counterclockwise_3x3_block_rotation()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 9, 2, 1, 3, 4, 8, 7 },
                { 4, 5, 7, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 1, 5, 3, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            RotationStep step = simulator.Rotate3x3Block(new Vector2Int(3, 1), RotationSense.CCW);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.Pivot, Is.EqualTo(new Vector2(3f, 1f)));
            Assert.That(step.RotationSense, Is.EqualTo(RotationSense.CCW));
            Assert.That(step.MovedTiles.Count, Is.EqualTo(8));
        }

        [Test]
        public void Test_swap_adjacent_tiles()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 9, 7, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            PermutationStep step = simulator.SwapTiles(new Vector2Int(3, 2), new Vector2Int(4, 2));

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.MovedTiles.Count, Is.EqualTo(2));
        }

        [Test]
        public void Test_swap_rows()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            PermutationStep step = simulator.SwapRows(2, 3);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.MovedTiles.Count, Is.EqualTo(18));
        }

        [Test]
        public void Test_swap_columns()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 1, 2, 1, 2, 3, 5 },
                { 2, 6, 1, 4, 6, 3, 5, 3, 4 },
                { 1, 5, 2, 2, 7, 4, 4, 4, 9 },
                { 1, 3, 2, 5, 2, 1, 3, 3, 3 },
                { 7, 1, 3, 3, 4, 1, 2, 2, 1 },
                { 8, 4, 7, 1, 6, 1, 2, 3, 6 },
                { 2, 3, 1, 9, 7, 3, 4, 8, 7 },
                { 4, 5, 5, 2, 6, 2, 5, 4, 1 },
                { 1, 2, 3, 1, 2, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, GetLevelSpecification());

            PermutationStep step = simulator.SwapColumns(3, 4);

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(step.MovedTiles.Count, Is.EqualTo(18));
        }

        [Test]
        public void Test_wildcard_tool()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Board expected = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            Vector2Int pos = new Vector2Int(2, 3);
            expected[pos].MakeWildcard();

            var simulator = new Simulator(board, GetLevelSpecification());

            WildcardStep step = simulator.CreateWildcard(pos);

            Assert.That(step.AffectedTiles.Count, Is.EqualTo(1));
            Assert.That(step.AffectedTiles[0], Is.SameAs(board[pos]));

            AssertThatBoardsAreEqual(board, expected);

            Assert.That(board[pos].Wildcard, Is.True);
        }

        private LevelSpecification GetLevelSpecification()
        {
            var levelSpec = ScriptableObject.CreateInstance<LevelSpecification>();

            levelSpec.InitialColorCount = 9;
            levelSpec.Size = new Vector2Int(9, 9);
            levelSpec.GuaranteedChain = 1;
            levelSpec.ChainsPerDifficultyIncrease = 1000;

            return levelSpec;
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

        private void AssertThatBoardsAreNotEqual(Board actual, Board expected)
        {
            foreach (int x in actual.GetXs())
                foreach (int y in actual.GetYs())
                {
                    if (actual[x, y] != expected[x, y])
                        return;
                }

            Assert.Fail("Boards were expect to be different, but were equal.");
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
