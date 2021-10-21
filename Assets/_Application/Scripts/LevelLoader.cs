using Array2DEditor;
using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.TutorialSystem;
using GMTK2020.UI;
using UnityEngine;

namespace GMTK2020
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Playback playback = null;
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private ChainCounter chainCounter = null;
        [SerializeField] private BoardManipulator predictionEditor = null;
        [SerializeField] private LevelSequence levelSequence = null;
        [SerializeField] private FixedLevelStartData fixedLevelStartData = null;

        public Level Level { get; private set; }

        private void Start()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            int levelIndex = GameProgression.CurrentLevelIndex;
            LevelSpecification levelSpec = levelSequence.Levels[levelIndex];

            int gameNumber = TutorialManager.GetGameCount();
            ++gameNumber;
            PlayerPrefs.SetInt(TutorialManager.GAME_COUNT_PREFS_KEY, gameNumber);
            if (gameNumber <= fixedLevelStartData.Levels.Length)
            {
                Level = SetUpFixedLevelStart(fixedLevelStartData.Levels[gameNumber-1]);
            }
            else
            {
                Level = new LevelGenerator(levelSpec).GenerateValidLevel();
            }

            chainCounter.RenderInitialResource(levelSpec.InitialResource);

            Simulator simulator = new Simulator(Level.Board, levelSpec);

            if (gameNumber <= fixedLevelStartData.Levels.Length)
                    simulator.SetFixedCracks(fixedLevelStartData.Levels[gameNumber-1].CrackedTiles);

            predictionEditor.Initialize(simulator, Level.Board);
            boardRenderer.RenderInitial(Level.Board);
            playback.Initialize(simulator);
        }

        private Level SetUpFixedLevelStart(FixedLevelStartData.FixedLevelStart levelData)
        {
            Array2DInt boardData = levelData.BoardData;
            Vector2Int gridSize = boardData.GridSize;

            var board = new Board(gridSize.x, gridSize.y);

            int[,] colors = boardData.GetCells();

            foreach (int y in board.GetYs())
                foreach (int x in board.GetXs())
                    board[x, y] = new Tile(colors[board.Height - y - 1, x]);

            return new Level(board);
        }
    }
}