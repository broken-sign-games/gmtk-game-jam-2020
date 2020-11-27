using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020.TutorialSystem
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialMap tutorialData = null;

        public static TutorialManager Instance { get; private set; }

        public delegate void TutorialHandler(Tutorial tutorial);
        public event TutorialHandler TutorialReady;
        public event TutorialHandler TutorialCompleted;

        private static readonly IEnumerable<TutorialID> allTutorials = Utility.GetEnumValues<TutorialID>();

        private TutorialID? activeTutorial;

        private Queue<TutorialID> queuedTutorials = new Queue<TutorialID>();

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

        private void Update()
        {
            if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.shiftKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
                ResetTutorial();
        }

        private void ResetTutorial()
        {
            foreach (TutorialID id in allTutorials)
            {
                PlayerPrefs.DeleteKey(TutorialIDToPrefsKey(id));
            }
        }

        public void ShowTutorialIfNew(TutorialID id)
        {
            if (TutorialWasShownAlready(id))
                return;

            ShowTutorial(id);
        }

        private void ShowTutorial(TutorialID id)
        {
            if (activeTutorial.HasValue)
            {
                queuedTutorials.Enqueue(id);
                return;
            }

            activeTutorial = id;
            TutorialReady?.Invoke(tutorialData.Map[id]);
        }

        private bool TutorialWasShownAlready(TutorialID id)
        {
            string prefsKey = TutorialIDToPrefsKey(id);
            return PlayerPrefs.GetInt(prefsKey, -1) >= 0;
        }

        public void CompleteActiveTutorial()
        {
            if (!activeTutorial.HasValue)
                return;

            TutorialID id = activeTutorial.Value;
            string prefsKey = TutorialIDToPrefsKey(id);
            PlayerPrefs.SetInt(prefsKey, 1);
            activeTutorial = null;
            TutorialCompleted(tutorialData.Map[id]);

            DequeueNextTutorialIfAvailable();
        }

        private void DequeueNextTutorialIfAvailable()
        {
            while (queuedTutorials.Count > 0 && TutorialWasShownAlready(queuedTutorials.Peek()))
                queuedTutorials.Dequeue();

            if (queuedTutorials.Count > 0)
                ShowTutorial(queuedTutorials.Dequeue());
        }

        private string TutorialIDToPrefsKey(TutorialID id) 
            => $"TutorialCompleted:{Enum.GetName(typeof(TutorialID), id)}";
    }
}