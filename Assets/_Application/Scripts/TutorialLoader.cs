﻿using Array2DEditor;
using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
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
        [SerializeField] private PatternRenderer patternRenderer = null;
        [SerializeField] private LevelSequence levelSequence = null;
        [SerializeField] private float delayAfterRenderingInitialBoard = 1f;
        [SerializeField] private float delayBetweenSimulations = 2f;
        [SerializeField] private float delayBetweenPredictions = 0.2f;
        [SerializeField] private float delayAfterPredictions = 1f;

        private HashSet<Vector2Int> levelPattern;
        private Board initialBoard;

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

            patternRenderer.RenderPattern(levelPattern);
            initialBoard = LoadTileGrid(levelSpec.TutorialBoard);

            RenderSimulatedTutorial(false);
        }

        private Board LoadTileGrid(Array2DInt tutorialBoard)
        {
            int[,] intGrid = tutorialBoard.GetCells();

            int width = tutorialBoard.GridSize.x;
            int height = tutorialBoard.GridSize.y;
            var board = new Board(width, height);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    int color = intGrid[height - y - 1, x];
                    if (color >= 0)
                        board[x, y] = new Tile(color);
                }

            return board;
        }

        private async void RenderSimulatedTutorial(bool renderPredictions)
        {
            boardRenderer.RenderInitial(initialBoard);

            await new WaitForSeconds(delayAfterRenderingInitialBoard);

            Simulator simulator = new Simulator(levelPattern);
            Simulation simulation = simulator.Simulate(initialBoard.DeepCopy(), true);

            if (renderPredictions)
            {
                int i = 1;
                foreach (SimulationStep step in simulation.Steps)
                {
                    foreach (Tile tile in step.MatchedTiles)
                    {
                        boardRenderer.UpdatePrediction(tile, i);
                        await new WaitForSeconds(delayBetweenPredictions);
                    }

                    ++i;
                }
            }

            await new WaitForSeconds(delayAfterPredictions - delayBetweenPredictions);

            LevelResult fakeResult = new LevelResult(5, new HashSet<Tile>(), new HashSet<Tile>());
            boardRenderer.KickOffRenderSimulation(simulation, fakeResult);
        }

        private async void OnSimulationRenderingCompleted()
        {
            await new WaitForSeconds(delayBetweenSimulations);

            RenderSimulatedTutorial(true);
        }
    }
}