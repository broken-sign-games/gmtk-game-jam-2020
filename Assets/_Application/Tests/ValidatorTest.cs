using System.Collections.Generic;
using System.Linq;
using GMTK2020;
using GMTK2020.Data;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ValidatorTest
    {
        private Validator validator = new Validator();

        [TestCaseSource(nameof(Cases))]
        public void TestAll(Case testCase)
        {
            LevelResult result = validator.ValidatePrediction(
                new Simulation(testCase.Simulated.Select((tiles) => Step(tiles)).ToList()),
                new Prediction(testCase.Predicted)
            );

            Assert.That(result.CorrectPredictions, Is.EqualTo(testCase.ExpectedSteps));
        }

        static object[] Cases()
        {
            var tile1 = new Tile(0, Vector2Int.zero);
            var tile2 = new Tile(2, Vector2Int.zero);
            var tile3 = new Tile(3, Vector2Int.zero);

            return new object[]
            {
                new Case(
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile1 },
                       new HashSet<Tile>() { tile2 },
                   },
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile1 },
                       new HashSet<Tile>() { tile2 },
                   },
                   expectedSteps: 2
                ),
                new Case(
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile1 },
                       new HashSet<Tile>() { tile2 },
                   },
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile1 },
                   },
                   expectedSteps: 1
                ),
                new Case(
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile2, tile3 },
                       new HashSet<Tile>() { tile1 },
                   },
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile2 },
                   },
                   expectedSteps: 0
                ),
                new Case(
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile1, tile2, tile3 },
                       new HashSet<Tile>() { },
                   },
                   new List<HashSet<Tile>>
                   {
                       new HashSet<Tile>() { tile1, tile2, tile3},
                   },
                   expectedSteps: 1
                ),
            };
        }

        public class Case
        {
            public List<HashSet<Tile>> Simulated { get; }
            public List<HashSet<Tile>> Predicted { get; }

            public int ExpectedSteps { get; }

            public Case(List<HashSet<Tile>> simulated, List<HashSet<Tile>> predicted, int expectedSteps)
            {
                Simulated = simulated;
                Predicted = predicted;
                ExpectedSteps = expectedSteps;
            }
        }

        private static SimulationStep Step(HashSet<Tile> matchedTiles)
        {
            return new SimulationStep(matchedTiles, new List<Tile>());
        }
    }
}
