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
        [SerializeField] private BoardManipulator predictionEditor = null;
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

            Simulator simulator = new Simulator(Level.Board, levelSpec.ColorCount);

            predictionEditor.Initialize(Level.Board, simulator);
            boardRenderer.RenderInitial(Level.Board);
            playback.Initialize(Level.Board, simulator);
        }
    }
}