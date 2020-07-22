using GMTK2020.Data;
using GMTK2020.Rendering;
using UnityEngine;

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

            // Need to be simulate here instead of in level loader, and simulation depends on predictions

            boardRenderer.KickOffRenderSimulation(level.Simulation, new LevelResult());
        }
    }
}