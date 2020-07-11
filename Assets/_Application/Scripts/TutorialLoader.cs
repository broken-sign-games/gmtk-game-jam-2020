using Array2DEditor;
using GMTK2020.Data;
using GMTK2020.Rendering;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GMTK2020
{
    public class TutorialLoader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tutorialText = null;
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private LevelSequence levelSequence = null;
        [SerializeField] private float delayAfterRenderingInitialBoard = 1f;
        [SerializeField] private float delayBetweenSimulations = 2f;

        private HashSet<Vector2Int> levelPattern;
        private Tile[,] initialGrid;

        private void Start()
        {
            boardRenderer.SimulationRenderingCompleted += OnSimulationRenderingCompleted;
            LoadTutorial();
        }

        private void OnDestroy()
        {
            boardRenderer.SimulationRenderingCompleted -= OnSimulationRenderingCompleted;
        }

        private void LoadTutorial()
        {
            int levelIndex = GameProgression.CurrentLevelIndex;
            LevelSpecification levelSpec = levelSequence.Levels[levelIndex];
            levelPattern = new HashSet<Vector2Int>(levelSpec.MatchingPattern);

            tutorialText.text = levelSpec.TutorialText;

            initialGrid = LoadTileGrid(levelSpec.TutorialBoard);

            RenderSimulatedTutorial();
        }

        private Tile[,] LoadTileGrid(Array2DInt tutorialBoard)
        {
            int[,] intGrid = tutorialBoard.GetCells();

            int width = tutorialBoard.GridSize.y;
            int height = tutorialBoard.GridSize.x;
            var grid = new Tile[width, height];

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    int color = intGrid[height - y - 1, x];
                    if (color >= 0)
                        grid[x, y] = new Tile(color);
                }

            return grid;
        }

        private async void RenderSimulatedTutorial()
        {
            boardRenderer.RenderInitial(initialGrid);

            await new WaitForSeconds(delayAfterRenderingInitialBoard);

            Simulator simulator = new Simulator(levelPattern);
            Simulation simulation = simulator.Simulate(initialGrid, true);
            boardRenderer.KickOffRenderSimulation(simulation, 5);
        }

        private async void OnSimulationRenderingCompleted()
        {
            await new WaitForSeconds(delayBetweenSimulations);

            RenderSimulatedTutorial();
        }
    }
}