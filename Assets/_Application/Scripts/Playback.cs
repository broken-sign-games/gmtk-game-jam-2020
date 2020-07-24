using GMTK2020.Data;
using GMTK2020.Rendering;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class Playback : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private PredictionEditor predictionEditor = null;
        [SerializeField] private LevelLoader levelLoader = null;

        private SoundManager SoundManager = null;

        private void Start()
        {
            SoundManager = FindObjectOfType<SoundManager>();
        }

        public void Run()
        {
            SoundManager?.PlayEffect(SoundManager.Effect.CLICK);

            Prediction prediction = predictionEditor.GetPredictions();
            Level level = levelLoader.Level;

            var rng = new Random();
            var simulator = new Simulator(new HashSet<Vector2Int>(level.MatchingPattern), rng);
            Simulation simulation = simulator.Simulate(level.Grid, prediction);

            boardRenderer.KickOffRenderSimulation(simulation, new LevelResult());
        }
    }
}