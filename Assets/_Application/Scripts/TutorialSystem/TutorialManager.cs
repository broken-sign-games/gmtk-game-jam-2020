using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private TaskCompletionSource<object> activeTutorialTCS;
        
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

        public Task ShowTutorialIfNewAsync(TutorialID id) 
            => ShowTutorialIfNewAsync(id, new List<GridRect>());

        public Task ShowTutorialIfNewAsync(TutorialID id, List<GridRect> dynamicInteractableRects)
        {
            if (TutorialWasAlreadyShown(id))
                return Task.CompletedTask;

            Tutorial tutorial = new Tutorial(tutorialMap[id]);

            if (tutorial.InteractableRects is null)
                tutorial.InteractableRects = dynamicInteractableRects;
            else
                tutorial.InteractableRects.AddRange(dynamicInteractableRects);

            return ShowTutorialAsync(tutorial);
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
            activeTutorialTCS.TrySetResult(null);
        }

        private Task ShowTutorialAsync(Tutorial tutorial)
        {
            if (activeTutorial != null)
                throw new InvalidOperationException("Can't show new tutorial while other tutorial is still in progress.");

            activeTutorial = tutorial;
            activeTutorialTCS = new TaskCompletionSource<object>();

            TutorialReady?.Invoke(tutorial);

            return activeTutorialTCS.Task;
        }

        private bool TutorialWasAlreadyShown(TutorialID id)
        {
            string prefsKey = TutorialIDToPrefsKey(id);
            return PlayerPrefs.GetInt(prefsKey, -1) >= 0;
        }

        private static string TutorialIDToPrefsKey(TutorialID id) 
            => $"TutorialCompleted:{Enum.GetName(typeof(TutorialID), id)}";
    }
}