using GMTK2020;
using GMTK2020.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace Tests
{
    public class ScoreKeeperTest
    {
        [TestCase(10, 3, ExpectedResult = 30)]
        [TestCase(10, 5, ExpectedResult = 50)]
        [TestCase(100, 3, ExpectedResult = 300)]
        public int Basic_match_scores_tiles_times_base_score(int baseScore, int nTiles)
        {
            var scoreKeeper = new ScoreKeeper(baseScore);

            var step = CreateMatchStep(1, nTiles);
            int matchScore = scoreKeeper.ScoreStep(step);

            return matchScore;
        }

        [TestCase(10, 3, 2, ExpectedResult = 60)]
        [TestCase(10, 5, 3, ExpectedResult = 150)]
        [TestCase(100, 3, 4, ExpectedResult = 1200)]
        public int Match_chains_escalate_score(int baseScore, int nTiles, int nSteps)
        {
            var scoreKeeper = new ScoreKeeper(baseScore);

            for (int i = 0; i < nSteps - 1; ++i)
                scoreKeeper.ScoreStep(CreateMatchStep(i+1, 0));

            var step = CreateMatchStep(nSteps, nTiles);
            int matchScore = scoreKeeper.ScoreStep(step);

            return matchScore;
        }

        [Test]
        public void Clean_up_step_scores_nothing()
        {
            var scoreKeeper = new ScoreKeeper(10);

            CleanUpStep step = new CleanUpStep(
                new List<MovedTile>()
                {
                    new MovedTile(),
                    new MovedTile(),
                }, 
                new HashSet<Tile>()
                {
                    new Tile(0),
                    new Tile(1),
                    new Tile(2),
                });
            
            int cleanUpScore = scoreKeeper.ScoreStep(step);

            Assert.That(cleanUpScore, Is.EqualTo(0));
        }

        [TestCase(10, 3, 1, ExpectedResult = 30)]
        [TestCase(10, 3, 2, ExpectedResult = 60)]
        [TestCase(10, 5, 3, ExpectedResult = 150)]
        [TestCase(100, 3, 4, ExpectedResult = 1200)]
        public int Clean_up_step_resets_match_chain(int baseScore, int nTiles, int nSteps)
        {
            var scoreKeeper = new ScoreKeeper(baseScore);

            scoreKeeper.ScoreStep(CreateMatchStep(1, 2));
            scoreKeeper.ScoreStep(CreateMatchStep(2, 3));

            scoreKeeper.ScoreStep(new CleanUpStep(new List<MovedTile>(), new HashSet<Tile>()));

            for (int i = 0; i < nSteps - 1; ++i)
                scoreKeeper.ScoreStep(CreateMatchStep(i + 1, 0));

            var step = CreateMatchStep(nSteps, nTiles);
            int matchScore = scoreKeeper.ScoreStep(step);

            return matchScore;
        }

        [Test]
        public void Score_keeper_tracks_total_score()
        {
            var scoreKeeper = new ScoreKeeper(10);

            scoreKeeper.ScoreStep(CreateMatchStep(1, 3)); // 3 * 10 * 1 = 30
            scoreKeeper.ScoreStep(CreateMatchStep(2, 4)); // 4 * 10 * 2 = 80

            scoreKeeper.ScoreStep(CreateCleanUpStep());

            scoreKeeper.ScoreStep(CreateMatchStep(1, 5)); // 5 * 10 * 1 = 50
            scoreKeeper.ScoreStep(CreateMatchStep(2, 2)); // 2 * 10 * 2 = 40

            scoreKeeper.ScoreStep(CreateCleanUpStep());

            scoreKeeper.ScoreStep(CreateMatchStep(1, 1)); // 1 * 10 * 1 = 10

            Assert.That(scoreKeeper.Score, Is.EqualTo(210));
        }

        private MatchStep CreateMatchStep(int chainLength, int nMatchedTiles)
        {
            var matchedTiles = new HashSet<Tile>();
            for (int x = 0; x < nMatchedTiles; ++x)
                matchedTiles.Add(new Tile(0, new Vector2Int(x, 0)));

            return new MatchStep(chainLength, matchedTiles, new List<MovedTile>(), new HashSet<Vector2Int>(), new HashSet<Vector2Int>());
        }

        private CleanUpStep CreateCleanUpStep()
        {
            return new CleanUpStep(new List<MovedTile>(), new HashSet<Tile>());
        }
    }
}
