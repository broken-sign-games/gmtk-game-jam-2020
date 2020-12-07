using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020
{
    public class Playback : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private ScoreRenderer scoreRenderer = null;
        [SerializeField] private Button runButton = null;
        [SerializeField] private Button retryButton = null;
        [SerializeField] private BoardManipulator boardManipulator = null;

        // TODO: This is probably not the best place to put this data.
        [SerializeField] private int baseScore = 100;

        private Board board;
        private ScoreKeeper scoreKeeper;

        private Simulator simulator;

        public void Initialize(Board initialBoard, Simulator simulator)
        {
            board = initialBoard;
            this.simulator = simulator;

            scoreKeeper = new ScoreKeeper(baseScore);
            scoreRenderer.SetScoreKeeper(scoreKeeper);

            boardManipulator.LastToolUsed += OnLastToolUsed;
        }

        private void OnDestroy()
        {
            boardManipulator.LastToolUsed -= OnLastToolUsed;
        }

        public async void KickOffPlayback()
        {
            // TODO: This should be triggered directly by the button instead
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

            await RunPlaybackAsync();
        }

        private async Task RunPlaybackAsync()
        {
            runButton.interactable = false;
            
            while (true)
            {
                SimulationStep step = simulator.SimulateNextStep();

                scoreKeeper.ScoreStep(step);
                // We might need to tie this into the board renderer 
                // to sync the update with the match animation.
                scoreRenderer.UpdateScore();

                if (step is MatchStep matchStep)
                    boardManipulator.RewardMatch(matchStep);

                await boardRenderer.AnimateSimulationStepAsync(step);

                if (step.FinalStep)
                    break;
            }

            if (simulator.FurtherMatchesPossible() || boardManipulator.AnyToolsAvailable())
                StartNewTurn();
            else
                EndLevel();
        }

        private void StartNewTurn()
        {
            boardManipulator.StartNewTurn();
            runButton.interactable = true;
        }

        public void OnLastToolUsed()
        {
            if (!simulator.FurtherMatchesPossible())
                EndLevel();
        }

        private void EndLevel()
        {
            runButton.interactable = false;
            scoreKeeper.UpdateHighscore();
            scoreRenderer.UpdateHighscore();
            retryButton.ActivateObject();
        }
    }
}