using GMTK2020.Data;
using GMTK2020.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK2020
{
    public class TutorialSystem : MonoBehaviour
    {
        [SerializeField] private TutorialPopup tutorialPopup = null;
        [SerializeField] private TutorialMap tutorialMap = null;

        private static readonly IEnumerable<TutorialID> allMessages = Utility.GetEnumValues<TutorialID>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
                ResetTutorial();
        }

        private void ResetTutorial()
        {
            foreach (TutorialID msg in allMessages)
            {
                PlayerPrefs.DeleteKey(Enum.GetName(typeof(TutorialID), msg));
            }
        }

        public void ShowTutorialIfNew(TutorialID msg)
        {
            string msgName = Enum.GetName(typeof(TutorialID), msg);
            if (PlayerPrefs.GetInt(msgName, -1) >= 0)
                return;

            ShowTutorial(msg);
        }

        public void ShowTutorial(TutorialID msg)
        {
            string msgName = Enum.GetName(typeof(TutorialID), msg);
            tutorialPopup.ShowTutorial(tutorialMap.Map[msg]);

            PlayerPrefs.SetInt(msgName, 1);
        }
    } 
}
