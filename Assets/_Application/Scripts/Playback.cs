using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.Rendering;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTK2020
{
    public class Playback : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private PredictionEditor predictionEditor = null;
        [SerializeField] private LevelLoader levelLoader = null;

        private Board board;
        private int colorCount;

        public void Initialize(Board initialBoard, int colorCount)
        {
            board = initialBoard;
            this.colorCount = colorCount;
        }

        public async void KickOffPlayback()
        {
            // TODO: This should be triggered directly by the button instead
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

            await RunPlaybackAsync();
        }

        private async Task RunPlaybackAsync()
        {
            var simulator = new Simulator(board, colorCount);
            
            while (true)
            {
                SimulationStep step = simulator.SimulateNextStep();

                await boardRenderer.AnimateSimulationStepAsync(step);

                if (step.FinalStep)
                    break;
            }
        }
    }
}