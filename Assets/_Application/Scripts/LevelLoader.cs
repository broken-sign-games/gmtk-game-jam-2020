using Array2DEditor;
using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.TutorialSystem;
using System;
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
        [SerializeField] private Array2DInt firstGameBoard = null;

        public Level Level { get; private set; }

        private void Start()
        {
            LoadLevel();
            TutorialManager.Instance.ShowTutorialIfNew(TutorialID.Welcome);
        }

        public void LoadLevel()
        {
            int levelIndex = GameProgression.CurrentLevelIndex;
            LevelSpecification levelSpec = levelSequence.Levels[levelIndex];

            if (IsFirstGame())
            {
                Level = SetUpFirstGameLevel();
            }
            else
            {
                Level = new LevelGenerator(levelSpec).GenerateValidLevel();
            }

            Simulator simulator = new Simulator(Level.Board, levelSpec.ColorCount);

            predictionEditor.Initialize(simulator);
            boardRenderer.RenderInitial(Level.Board);
            playback.Initialize(Level.Board, simulator);
        }

        private bool IsFirstGame() 
            => PlayerPrefs.GetInt(TutorialManager.FIRST_GAME_PREFS_KEY, -1) < 0;

        private Level SetUpFirstGameLevel()
        {
            Vector2Int gridSize = firstGameBoard.GridSize;

            var board = new Board(gridSize.x, gridSize.y);

            int[,] colors = firstGameBoard.GetCells();

            foreach (int y in board.GetYs())
                foreach (int x in board.GetXs())
                    board[x, y] = new Tile(colors[board.Height - y - 1, x]);

            // TODO: We probably want to tie this to a certain tutorial message instead
            PlayerPrefs.SetInt(TutorialManager.FIRST_GAME_PREFS_KEY, 1);

            return new Level(board);
        }
    }
}