using GMTK2020.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK2020
{
    public class SessionMetrics : MonoBehaviour
    {
        public float SessionTime { get; private set; }
        public int MaxChainLength { get; private set; }
        public int MaxMatchSize { get; private set; }
        public int MistakeCount { get; private set; }

        private float startTime;

        private bool gameEnded;

        private Dictionary<Tool, int> toolUnlocks;
        private Dictionary<Tool, int> toolUses;

        private void Awake()
        {
            startTime = Time.time;
            toolUnlocks = new Dictionary<Tool, int>();
            toolUses = new Dictionary<Tool, int>();

            foreach (var tool in Utility.GetEnumValues<Tool>())
            {
                toolUnlocks[tool] = 0;
                toolUses[tool] = 0;
            }
        }

        private void Update()
        {
            if (!gameEnded)
                SessionTime = Time.time - startTime;
        }

        public int GetToolUnlocks(Tool tool)
            => toolUnlocks[tool];

        public int GetToolUses(Tool tool)
            => toolUses[tool];

        public void RegisterSimulationStep(SimulationStep step)
        {
            switch (step)
            {
            case MatchStep matchStep:
                if (matchStep.ChainLength > MaxChainLength)
                    MaxChainLength = matchStep.ChainLength;

                if (matchStep.MatchedTiles.Count > MaxMatchSize)
                    MaxMatchSize = matchStep.MatchedTiles.Count;
                break;
            case CleanUpStep cleanUpStep:
                MistakeCount += cleanUpStep.InertTiles.Count(tile => tile.Marked);
                break;
            }
        }

        public void RegisterToolUse(Tool tool)
            => ++toolUses[tool];

        public void RegisterToolRewards(List<Tool> tools)
        {
            foreach (Tool tool in tools)
                ++toolUnlocks[tool];
        }

        public void RegisterEndOfGame()
        {
            gameEnded = true;
        }
    }
}
