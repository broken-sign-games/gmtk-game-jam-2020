using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Audio
{
    public class TutorialSystem : MonoBehaviour
    {
        [SerializeField] private TutorialMap tutorialData = null;

        public static TutorialSystem Instance { get; private set; }

        public delegate void TutorialHandler(Tutorial tutorial);
        public event TutorialHandler TutorialReady;

        private static readonly IEnumerable<TutorialID> allTutorials = Utility.GetEnumValues<TutorialID>();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Two instances of TutorialSystem detected. Deleting this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public static void ResetTutorial()
        {
            foreach (TutorialID msg in allTutorials)
            {
                PlayerPrefs.DeleteKey(Enum.GetName(typeof(TutorialID), msg));
            }
        }

        public void ShowTutorialIfNew(TutorialID id)
        {
            if (TutorialWasShownAlready(id))
                return;

            TutorialReady?.Invoke(tutorialData.Map[id]);
        }
        private bool TutorialWasShownAlready(TutorialID id)
        {
            string prefsKey = TutorialIDToPrefsKey(id);
            return PlayerPrefs.GetInt(prefsKey, -1) >= 0;
        }

        public void CompleteTutorial(TutorialID id)
        {
            string prefsKey = TutorialIDToPrefsKey(id);
            PlayerPrefs.SetInt(prefsKey, 1);
        }

        private string TutorialIDToPrefsKey(TutorialID id) 
            => $"TutorialCompleted:{Enum.GetName(typeof(TutorialID), id)}";
    }
}