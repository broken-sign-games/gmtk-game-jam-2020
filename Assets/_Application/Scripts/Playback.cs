using GMTK2020.Data;
using GMTK2020.Rendering;
using System;
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
        [SerializeField] private LevelLoader levelLoader = null;
        [SerializeField] private Button retryButton = null;
        [SerializeField] private Button playButton = null;
        [SerializeField] private BoardManipulator boardManipulator = null;

        private SoundManager soundManager = null;

        private Tile[,] nextGrid;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();

            boardRenderer.SimulationRenderingCompleted += OnSimulationRenderingCompleted;
        }

        private void OnDestroy()
        {
            boardRenderer.SimulationRenderingCompleted -= OnSimulationRenderingCompleted;
        }

        public void Run()
        {
            soundManager?.PlayEffect(SoundManager.Effect.CLICK);

            boardManipulator.LockBoard();
            Prediction prediction = predictionEditor.GetPredictions();
            Level level = levelLoader.Level;

            var rng = new Random();
            var simulator = new Simulator(new HashSet<Vector2Int>(level.MatchingPattern), rng, true, levelLoader.ScoreKeeper);
            Simulation simulation = nextGrid is null 
                ? simulator.Simulate(level.Grid, prediction)
                : simulator.Simulate(nextGrid, prediction);
            nextGrid = simulation.FinalGrid;

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
            }
            else
            {
                GameOver();
            }
        }

        public void CheckForGameOver()
        {
            var rng = new Random();
            var simulator = new Simulator(new HashSet<Vector2Int>(levelLoader.Level.MatchingPattern), rng, true, levelLoader.ScoreKeeper);

            if (!simulator.CheckIfFurtherMatchesPossible(nextGrid))
                GameOver();
        }

        private void GameOver()
        {
            playButton.interactable = false;
            boardManipulator.LockBoard();
            levelLoader.ScoreKeeper.UpdateHighscore();
            retryButton.gameObject.SetActive(true);
        }
    }
}