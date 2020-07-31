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

        private SoundManager SoundManager = null;

        private Tile[,] nextGrid;

        private void Start()
        {
            SoundManager = FindObjectOfType<SoundManager>();

            boardRenderer.SimulationRenderingCompleted += OnSimulationRenderingCompleted;
        }

        private void OnDestroy()
        {
            boardRenderer.SimulationRenderingCompleted -= OnSimulationRenderingCompleted;
        }

        public void Run()
        {
            SoundManager?.PlayEffect(SoundManager.Effect.CLICK);

            Prediction prediction = predictionEditor.GetPredictions();
            Level level = levelLoader.Level;

            var rng = new Random();
            var simulator = new Simulator(new HashSet<Vector2Int>(level.MatchingPattern), rng);
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
                predictionEditor.gameObject.SetActive(true);
                playButton.interactable = true;
            }
            else
            {
                retryButton.gameObject.SetActive(true);
            }
        }
    }
}