using GMTK2020.Data;
using GMTK2020.Rendering;
using TMPro;
using UnityEngine;

namespace GMTK2020
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Playback playback = null;
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private PredictionEditor predictionEditor = null;
        [SerializeField] private LevelSequence levelSequence = null;

        public Level Level { get; private set; }

        private void Awake()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            int levelIndex = GameProgression.CurrentLevelIndex;
            LevelSpecification levelSpec = levelSequence.Levels[levelIndex];

            Level = new LevelGenerator(levelSpec).GenerateValidLevel();

            predictionEditor.Initialize(Level.Board);
            boardRenderer.RenderInitial(Level.Board);
            playback.Initialize(Level.Board, levelSpec.ColorCount);
        }
    }
}