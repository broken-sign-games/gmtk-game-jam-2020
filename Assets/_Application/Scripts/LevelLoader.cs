using Array2DEditor;
using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.TutorialSystem;
using UnityEngine;

namespace GMTK2020
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Playback playback = null;
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private BoardManipulator predictionEditor = null;
        [SerializeField] private LevelSequence levelSequence = null;
        [SerializeField] private FixedLevelStartData fixedLevelStartData = null;

        public Level Level { get; private set; }

        private void Start()
        {
            LoadLevel();
            TutorialManager.Instance.ShowTutorialIfNew(TutorialID.OpenVials);
        }

        public void LoadLevel()
        {
            int levelIndex = GameProgression.CurrentLevelIndex;
            LevelSpecification levelSpec = levelSequence.Levels[levelIndex];

            int gameNumber = GetGameNumber();
            if (gameNumber < fixedLevelStartData.Levels.Length)
            {
                Level = SetUpFixedLevelStart(fixedLevelStartData.Levels[gameNumber]);
            }
            else
            {
                Level = new LevelGenerator(levelSpec).GenerateValidLevel();
            }

            Simulator simulator = new Simulator(Level.Board, levelSpec);

            if (gameNumber < fixedLevelStartData.Levels.Length)
                    simulator.SetFixedCracks(fixedLevelStartData.Levels[levelIndex].CrackedTiles);

            predictionEditor.Initialize(simulator);
            boardRenderer.RenderInitial(Level.Board);
            playback.Initialize(Level.Board, simulator);

            PlayerPrefs.SetInt(TutorialManager.GAME_COUNT_PREFS_KEY, gameNumber+1);
        }

        private int GetGameNumber() 
            => PlayerPrefs.GetInt(TutorialManager.GAME_COUNT_PREFS_KEY, 0);

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