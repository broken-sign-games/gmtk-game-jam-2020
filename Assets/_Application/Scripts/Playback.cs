using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.TutorialSystem;
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
        [SerializeField] private CrackCounter crackCounter = null;

        // TODO: This is probably not the best place to put this data.
        [SerializeField] private int baseScore = 100;

        private ScoreKeeper scoreKeeper;
        private Simulator simulator;
        private TutorialManager tutorialManager;

        private int turnCount;
        private bool reactionStarted;
        private bool gameEnded;

        public void Initialize(Simulator simulator)
        {
            this.simulator = simulator;

            tutorialManager = TutorialManager.Instance;

            scoreKeeper = new ScoreKeeper(baseScore);
            scoreRenderer.SetScoreKeeper(scoreKeeper);
            crackCounter.SetAvoidedCracks(0);
            crackCounter.SetMaxCracks(simulator.CracksPerChain);

            boardManipulator.LastToolUsed += OnLastToolUsed;

            KickOffGameplayLoop();
        }

        private void OnDestroy()
        {
            boardManipulator.LastToolUsed -= OnLastToolUsed;
        }

        public void StartReaction()
        {
            // TODO: This should be triggered directly by the button instead
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

            reactionStarted = true;
        }

        private async void KickOffGameplayLoop()
        {
            await RunGameplayLoopAsync();
        }

        private async Task RunGameplayLoopAsync()
        {
            turnCount = 0;

            while (true)
            {
                reactionStarted = false;

                await ShowInteractiveTutorialsAsync();

                await Awaiters.Until(() => reactionStarted || gameEnded);

                if (gameEnded)
                    break;

                await PlayBackReactionAsync();

                boardManipulator.MakeToolsAvailable();

                if (!simulator.FurtherMatchesPossible() && !boardManipulator.AnyToolsAvailable())
                {
                    EndGame();
                    break;
                }

                StartNewTurn();    
            }
        }

        private async Task ShowInteractiveTutorialsAsync()
        {
            int gameCount = TutorialManager.GetGameCount();

            switch (gameCount)
            {
            case 1:
                await ShowFirstGameTutorialSequenceAsync();
                break;
            case 2:
                await ShowSecondGameTutorialAsync();
                break;
            }
        }

        private async Task ShowFirstGameTutorialSequenceAsync()
        {
            switch (turnCount)
            {
            case 0:
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.OpenVials);
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.StartReaction);
                break;
            case 1:
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.SwapTiles);
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.PredictChains);
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.StartChainReaction);
                break;
            case 2:
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.RemoveTileTool);
                await tutorialManager.ShowTutorialIfNewAsync(TutorialID.EndOfMainTutorial);
                break;
            }
        }

        private async Task ShowSecondGameTutorialAsync()
        {
            if (turnCount > 0)
                return;

            await tutorialManager.ShowTutorialIfNewAsync(TutorialID.OmittingVials);
        }

        private async Task PlayBackReactionAsync()
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

                crackCounter.SetAvoidedCracks(simulator.ChainLength);
                
                await boardRenderer.AnimateSimulationStepAsync(step);

                if (step.FinalStep)
                    break;
            }
        }

        private void StartNewTurn()
        {
            ++turnCount;

            runButton.interactable = true;
            crackCounter.SetAvoidedCracks(0);
            crackCounter.SetMaxCracks(simulator.CracksPerChain);
        }

        private void OnLastToolUsed()
        {
            if (!simulator.FurtherMatchesPossible())
                EndGame();
        }

        private void EndGame()
        {
            gameEnded = true;

            runButton.interactable = false;
            scoreKeeper.UpdateHighscore();
            scoreRenderer.UpdateHighscore();
            retryButton.ActivateObject();
        }
    }
}