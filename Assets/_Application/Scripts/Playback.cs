using GMTK2020.Data;
using GMTK2020.Rendering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace GMTK2020
{
    public class Playback : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private PredictionEditor predictionEditor = null;
        [SerializeField] private LevelSpecification levelSpec = null;
        [SerializeField] private Button retryButton = null;
        [SerializeField] private Button playButton = null;
        [SerializeField] private BoardManipulator boardManipulator = null;
        [SerializeField] private TutorialSystem tutorial = null;

        private Level level;
        private ScoreKeeper scoreKeeper;

        private SoundManager soundManager;

        private Tile[,] nextGrid;

        private TutorialSystem.TutorialMessage queuedTutorialMessage;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();

            boardRenderer.SimulationRenderingCompleted += OnSimulationRenderingCompleted;

            scoreKeeper = new ScoreKeeper();
            LoadLevel();
        }

        private void OnDestroy()
        {
            boardRenderer.SimulationRenderingCompleted -= OnSimulationRenderingCompleted;
        }

        public void LoadLevel()
        {
            HashSet<Vector2Int> levelPattern = new HashSet<Vector2Int>(levelSpec.MatchingPattern);

            // TODO: Use RNG with same seed during level generation and playback.
            var rng = new Random();
            Simulator simulator = new Simulator(levelPattern, rng, false);

            level = new LevelGenerator(levelSpec, simulator).GenerateValidLevel();

            boardManipulator.Initialize(level.Grid);
            predictionEditor.Initialize(level.Grid);
            boardRenderer.RenderInitial(level.Grid, scoreKeeper);

            nextGrid = level.Grid;

            tutorial.ShowTutorialIfNew(TutorialSystem.TutorialMessage.FirstMatch);
        }

        public void Run()
        {
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);

            boardManipulator.LockBoard();
            Prediction prediction = predictionEditor.GetPredictions();
            
            var rng = new Random();
            var simulator = new Simulator(new HashSet<Vector2Int>(level.MatchingPattern), rng, true, scoreKeeper);
            Simulation simulation = simulator.Simulate(nextGrid, prediction);
            nextGrid = simulation.FinalGrid;

            if (simulation.ClearBoardStep.ExtraneousPredictions.Count > 0)
                queuedTutorialMessage = TutorialSystem.TutorialMessage.StoneBlocks;
            else if (simulation.Steps.Count > 1)
                queuedTutorialMessage = TutorialSystem.TutorialMessage.ChainRewards;
            else
                queuedTutorialMessage = TutorialSystem.TutorialMessage.SubsequentMatch;


            boardRenderer.KickOffRenderSimulation(simulation);
        }

        private void OnSimulationRenderingCompleted(bool furtherMatchesPossible)
        {
            if (furtherMatchesPossible)
            {
                predictionEditor.Initialize(nextGrid);
                boardManipulator.Initialize(nextGrid);
                predictionEditor.gameObject.SetActive(true);
                playButton.interactable = true;
                boardManipulator.UnlockBoard();

                tutorial.ShowTutorialIfNew(queuedTutorialMessage);
            }
            else
            {
                GameOver();
            }
        }

        public void CheckForGameOver()
        {
            var rng = new Random();
            var simulator = new Simulator(new HashSet<Vector2Int>(level.MatchingPattern), rng, true, scoreKeeper);

            if (!simulator.CheckIfFurtherMatchesPossible(nextGrid))
                GameOver();
        }

        private void GameOver()
        {
            playButton.interactable = false;
            boardManipulator.LockBoard();
            scoreKeeper.UpdateHighscore();
            retryButton.gameObject.SetActive(true);
        }
    }
}