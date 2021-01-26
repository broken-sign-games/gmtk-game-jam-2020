using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class InfoBox : MonoBehaviour
    {
        [SerializeField] private BoardManipulator boardManipulator = null;
        [SerializeField] private ToolData toolData = null;
        [SerializeField] private TextMeshProUGUI infoText = null;
        [SerializeField] private Button dismissTutorialButton = null;

        private TutorialManager tutorialManager;

        private Tool activeTool = Tool.ToggleMarked;
        private Tutorial activeTutorial = null;

        private void Start()
        {
            tutorialManager = TutorialManager.Instance;

            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;

            boardManipulator.ActiveToolChanged += OnActiveToolChanged;
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;

            boardManipulator.ActiveToolChanged -= OnActiveToolChanged;
        }

        public void DismissTutorial()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            tutorialManager.CompleteActiveTutorial();
        }

        private void OnTutorialReady(Tutorial tutorial)
        {
            activeTutorial = tutorial;
            UpdateInfoBox();
        }

        private void OnTutorialCompleted(Tutorial tutorial)
        {
            activeTutorial = null;
            UpdateInfoBox();
        }

        private void OnActiveToolChanged(Tool tool)
        {
            activeTool = tool;
            UpdateInfoBox();
        }

        private void UpdateInfoBox()
        {
            if (activeTutorial is null)
                RenderTooltip();
            else
                RenderTutorialMessage();
        }

        private void RenderTutorialMessage()
        {
            infoText.text = activeTutorial.Message;
            if (activeTutorial.ShowDismissButton)
                dismissTutorialButton.ActivateObject();
        }

        private void RenderTooltip()
        {
            infoText.text = toolData.Map[activeTool].Tooltip;
            dismissTutorialButton.DeactivateObject();
        }
    } 
}
