using GMTK2020.Audio;
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

        public void Run()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

            Prediction prediction = predictionEditor.GetPredictions();
            Level level = levelLoader.Level;
            
            //boardRenderer.KickOffRenderSimulation(level.Simulation, levelResult);
        }
    }
}