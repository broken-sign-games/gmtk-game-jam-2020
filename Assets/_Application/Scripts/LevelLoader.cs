using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private PatternRenderer patternRenderer = null;
        [SerializeField] private PredictionEditor predictionEditor = null;
        [SerializeField] private LevelSequence levelSequence = null;

        public Level Level { get; private set; }

        private void Start()
        {
            RunIt();
        }

        public void RunIt()
        {
            LevelSpecification levelSpec = levelSequence.Levels[GameProgression.CurrentLevelIndex];
            HashSet<Vector2Int> levelPattern = new HashSet<Vector2Int>(levelSpec.MatchingPattern);

            Simulator simulator = new Simulator(levelPattern);

            Level = new LevelGenerator(levelSpec, simulator).GenerateValidLevel();

            predictionEditor.Initialize(Level.Grid);
            boardRenderer.RenderInitial(Level.Grid);
            patternRenderer.RenderPattern(levelPattern);
        }
    } 
}