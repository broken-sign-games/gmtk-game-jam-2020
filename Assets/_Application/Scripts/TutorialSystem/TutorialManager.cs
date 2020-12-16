using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020.TutorialSystem
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialData tutorialData = null;

        private Dictionary<TutorialID, Tutorial> tutorialMap;

        public static TutorialManager Instance { get; private set; }

        public delegate void TutorialHandler(Tutorial tutorial);
        public event TutorialHandler TutorialReady;
        public event TutorialHandler TutorialCompleted;

        private static readonly IEnumerable<TutorialID> allTutorials = Utility.GetEnumValues<TutorialID>();

        private Tutorial activeTutorial;

        private Queue<Tutorial> queuedTutorials = new Queue<Tutorial>();
        
        public static readonly string GAME_COUNT_PREFS_KEY = "FirstGameCompleted";

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Two instances of TutorialSystem detected. Deleting this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            tutorialMap = tutorialData.Tutorials.ToDictionary(tut => tut.ID);
        }

        private void Update()
        {
            if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.shiftKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
                ResetTutorial();
        }

        private static void ResetTutorial()
        {
            Debug.Log("Resetting tutorial");
            foreach (TutorialID id in allTutorials)
            {
                PlayerPrefs.DeleteKey(TutorialIDToPrefsKey(id));
            }
            PlayerPrefs.DeleteKey(GAME_COUNT_PREFS_KEY);
        }

        public static int GetGameCount()
            => PlayerPrefs.GetInt(GAME_COUNT_PREFS_KEY, 0);

        public void ShowTutorialIfNew(TutorialID id) 
            => ShowTutorialIfNew(id, new List<GridRect>());

        public void ShowTutorialIfNew(TutorialID id, List<GridRect> interactableRects)
        {
            if (TutorialWasShownAlready(id))
                return;

            ShowTutorial(id, interactableRects);
        }

        private void ShowTutorial(TutorialID id, List<GridRect> interactableRects)
        {
            Tutorial tutorial = tutorialMap[id];
            tutorial.InteractableRects = interactableRects;

            ShowTutorial(tutorial);
        }

        private void ShowTutorial(Tutorial tutorial)
        {
            if (activeTutorial != null)
            {
                queuedTutorials.Enqueue(tutorial);
                return;
            }

            activeTutorial = tutorial;
            TutorialReady?.Invoke(tutorial);
        }

        private bool TutorialWasShownAlready(TutorialID id)
        {
            string prefsKey = TutorialIDToPrefsKey(id);
            return PlayerPrefs.GetInt(prefsKey, -1) >= 0;
        }

        public void CompleteActiveTutorial()
        {
            if (activeTutorial is null)
                return;

            string prefsKey = TutorialIDToPrefsKey(activeTutorial.ID);
            PlayerPrefs.SetInt(prefsKey, 1);
            Tutorial completedTutorial = activeTutorial;
            activeTutorial = null;
            TutorialCompleted(completedTutorial);

            DequeueNextTutorialIfAvailable();
        }

        private void DequeueNextTutorialIfAvailable()
        {
            while (queuedTutorials.Count > 0 && TutorialWasShownAlready(queuedTutorials.Peek().ID))
                queuedTutorials.Dequeue();

            if (queuedTutorials.Count > 0)
                ShowTutorial(queuedTutorials.Dequeue());
        }

        private static string TutorialIDToPrefsKey(TutorialID id) 
            => $"TutorialCompleted:{Enum.GetName(typeof(TutorialID), id)}";
    }
}