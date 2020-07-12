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

            Validator validator = new Validator();
            Prediction prediction = predictionEditor.GetPredictions();
            Level level = levelLoader.Level;
            LevelResult levelResult = validator.ValidatePrediction(level.Simulation, prediction);

            boardRenderer.KickOffRenderSimulation(level.Simulation, levelResult);
        }
    }
}