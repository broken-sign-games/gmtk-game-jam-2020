using GMTK2020.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GMTK2020
{
    public class TutorialSystem : MonoBehaviour
    {
        [SerializeField] private TutorialPopup tutorialPopup = null;

        public enum TutorialMessage
        {
            FirstMatch,
            SubsequentMatch,
            ChainRewards,
            StoneBlocks,
        }

        private static readonly TutorialMessage[] allMessages = Enum.GetValues(typeof(TutorialMessage)) as TutorialMessage[];

        private static readonly Dictionary<TutorialMessage, string> messages = new Dictionary<TutorialMessage, string>()
        {
            { TutorialMessage.FirstMatch, "Mark three matching tiles in a row. Then click \"Match\"." },
            { TutorialMessage.SubsequentMatch, "You can also mark matches that will form as a result of earlier matches disappearing." },
            { TutorialMessage.ChainRewards, "For each match after the first one, one row is cleared from the board and you can swap one pair of adjacent tiles. Use swaps to set up longer chain reactions!" },
            { TutorialMessage.StoneBlocks, "If any marked tiles remain when the chain reaction stops, they will be turned to stone and can no longer match." },
        };

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
                ResetTutorial();
        }

        private void ResetTutorial()
        {
            foreach (TutorialMessage msg in allMessages)
            {
                PlayerPrefs.DeleteKey(Enum.GetName(typeof(TutorialMessage), msg));
            }
        }

        public void ShowTutorialIfNew(TutorialMessage msg)
        {
            string msgName = Enum.GetName(typeof(TutorialMessage), msg);
            if (PlayerPrefs.GetInt(msgName, -1) >= 0)
                return;

            ShowTutorial(msg);
        }

        public void ShowTutorial(TutorialMessage msg)
        {
            string msgName = Enum.GetName(typeof(TutorialMessage), msg);
            tutorialPopup.ShowMessage(messages[msg]);

            PlayerPrefs.SetInt(msgName, 1);
        }
    } 
}
