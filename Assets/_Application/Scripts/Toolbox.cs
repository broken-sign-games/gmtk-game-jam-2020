using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020
{
    public class Toolbox
    {
        private Simulator simulator;
        private ToolData toolData;

        private Dictionary<Tool, int> availableToolUses;
        private Dictionary<MatchShape, Tool> shapeRewards;
        private Dictionary<Tool, int> requiredChainLength;
        private Dictionary<Tool, int> awardedToolsForCurrentChainLength;

        bool toolUsedThisTurn = true;

        public Toolbox(ToolData toolData, Simulator simulator)
        {
            this.toolData = toolData;
            this.simulator = simulator;

            availableToolUses = new Dictionary<Tool, int>();
            shapeRewards = new Dictionary<MatchShape, Tool>();
            requiredChainLength = new Dictionary<Tool, int>();
            awardedToolsForCurrentChainLength = new Dictionary<Tool, int>();

            foreach ((Tool tool, ToolData.SingleToolData data) in toolData.Map)
            {
                availableToolUses[tool] = data.InitialUses;

                if (data.AwardedFor != MatchShape.None)
                {
                    if (shapeRewards.ContainsKey(data.AwardedFor))
                        throw new ArgumentOutOfRangeException(nameof(toolData), "Multiple tools associated with the same shape");

                    shapeRewards[data.AwardedFor] = tool;
                }

                requiredChainLength[tool] = 1;
                awardedToolsForCurrentChainLength[tool] = 0;
            }
        }

        public int GetAvailableUses(Tool tool)
            => availableToolUses[tool];

        public int GetRequiredChainLength(Tool tool)
            => requiredChainLength[tool];

        public int GetAwardedToolsForCurrentChainLength(Tool tool)
            => awardedToolsForCurrentChainLength[tool];

        public int GetAvailableToolUsesForChainLength(int chainLength)
        {
            switch (toolData.RewardStrategy)
            {
            case RewardStrategy.OnePerLength: return 1;
            case RewardStrategy.NPerLength: return chainLength;
            case RewardStrategy.NAtOncePerLength: return chainLength;
            case RewardStrategy.PermanentUnlock: return -1;
            case RewardStrategy.OnePerMatch: return -1;
            default:
                throw new InvalidOperationException("Unknown reward strategy.");
            }
        }

        public bool IsToolAvailable(Tool tool)
        {
            if (tool == Tool.ToggleMarked)
                return true;

            if (toolUsedThisTurn)
                return false;

            return availableToolUses[tool] != 0;
        }

        public bool AnyToolsAvailable()
        {
            if (toolUsedThisTurn)
                return false;

            foreach ((Tool tool, int uses) in availableToolUses)
                if (tool != Tool.ToggleMarked && uses != 0)
                    return true;

            return false;
        }

        public void MakeToolsAvailable()
        {
            toolUsedThisTurn = false;
        }

        public SimulationStep UseTool(Tool tool, Vector2Int gridPos, RotationSense rotSense = RotationSense.CW)
        {
            switch (tool)
            {
            case Tool.SwapTiles:
            case Tool.SwapLines:
                throw new ArgumentOutOfRangeException(nameof(tool));
            }

            if (availableToolUses[tool] == 0)
                throw new InvalidOperationException("No tool uses available.");

            SimulationStep step = null;

            switch (tool)
            {
            case Tool.ToggleMarked:
                step = simulator.TogglePrediction(gridPos); break;
            case Tool.RemoveTile:
                step = simulator.RemoveTile(gridPos); break;
            case Tool.RefillInert:
                step = simulator.RefillTile(gridPos); break;
            case Tool.Bomb:
                step = simulator.RemoveBlock(gridPos); break;
            case Tool.PlusBomb:
                step = simulator.RemovePlus(gridPos); break;
            case Tool.RemoveRow:
                step = simulator.RemoveRow(gridPos.y); break;
            case Tool.RemoveColor:
                step = simulator.RemoveColor(gridPos); break;
            case Tool.Rotate3x3:
                step = simulator.Rotate3x3Block(gridPos, rotSense); break;
            case Tool.CreateWildcard:
                step = simulator.CreateWildcard(gridPos); break;
            }

            if (availableToolUses[tool] > 0)
                --availableToolUses[tool];

            if (tool != Tool.ToggleMarked)
                toolUsedThisTurn = true;

            return step;
        }

        public SimulationStep UseSwapTool(Tool tool, Vector2Int from, Vector2Int to)
        {
            switch (tool)
            {
            case Tool.ToggleMarked:
            case Tool.RemoveTile:
            case Tool.RefillInert:
            case Tool.Bomb:
            case Tool.PlusBomb:
            case Tool.RemoveRow:
            case Tool.RemoveColor:
            case Tool.Rotate3x3:
            case Tool.CreateWildcard:
                throw new ArgumentOutOfRangeException(nameof(tool));
            }

            if (availableToolUses[tool] == 0)
                throw new InvalidOperationException("No tool uses available.");

            SimulationStep step = null;

            switch (tool)
            {
            case Tool.SwapTiles:
                step = simulator.SwapTiles(from, to); break;
            case Tool.SwapLines:
                step = from.x == to.x
                    ? simulator.SwapRows(from.y, to.y)
                    : simulator.SwapColumns(from.x, to.x);
                break;
            }

            if (availableToolUses[tool] > 0)
                --availableToolUses[tool];

            toolUsedThisTurn = true;

            return step;
        }

        public List<Tool> RewardMatches(MatchStep step)
        {
            var previousToolUses = new Dictionary<Tool, int>(availableToolUses);

            foreach (Vector2Int match in step.LeftEndsOfHorizontalMatches)
            {
                RewardShape(step.ChainLength, MatchShape.Row3);

                if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(1, 0)))
                    RewardShape(step.ChainLength, MatchShape.Row4);

                if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(2, 0)))
                    RewardShape(step.ChainLength, MatchShape.Row5);

                if (step.BottomEndsOfVerticalMatches.Contains(match))
                {
                    RewardShape(step.ChainLength, MatchShape.L);

                    if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(0, 2)))
                    {
                        RewardShape(step.ChainLength, MatchShape.U);

                        if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, 0)))
                            RewardShape(step.ChainLength, MatchShape.O);
                    }

                    if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, 0)))
                        RewardShape(step.ChainLength, MatchShape.U);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, 0)))
                {
                    RewardShape(step.ChainLength, MatchShape.L);

                    if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(0, 2)))
                        RewardShape(step.ChainLength, MatchShape.U);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, -2)))
                {
                    RewardShape(step.ChainLength, MatchShape.L);

                    if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -2)))
                        RewardShape(step.ChainLength, MatchShape.U);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -2)))
                    RewardShape(step.ChainLength, MatchShape.L);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(1, 0)))
                {
                    RewardShape(step.ChainLength, MatchShape.T);

                    if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(0, 2)))
                        RewardShape(step.ChainLength, MatchShape.H);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(1, -2)))
                    RewardShape(step.ChainLength, MatchShape.T);


                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, -1)))
                {
                    RewardShape(step.ChainLength, MatchShape.T);

                    if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -1)))
                        RewardShape(step.ChainLength, MatchShape.H);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -1)))
                    RewardShape(step.ChainLength, MatchShape.T);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(1, -1)))
                    RewardShape(step.ChainLength, MatchShape.Plus);
            }

            foreach (Vector2Int match in step.BottomEndsOfVerticalMatches)
            {
                RewardShape(step.ChainLength, MatchShape.Row3);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, 1)))
                    RewardShape(step.ChainLength, MatchShape.Row4);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, 2)))
                    RewardShape(step.ChainLength, MatchShape.Row5);
            }

            var newTools = new List<Tool>();
            foreach ((Tool tool, int uses) in availableToolUses)
                for (int i = previousToolUses[tool]; i < uses; ++i)
                    newTools.Add(tool);

            return newTools;
        }

        private void RewardShape(int chainLength, MatchShape shape)
        {
            if (!shapeRewards.ContainsKey(shape))
                return;

            Tool tool = shapeRewards[shape];

            if (availableToolUses[tool] < 0)
                return;

            if (chainLength < requiredChainLength[tool])
                return;

            switch (toolData.RewardStrategy)
            {
            case RewardStrategy.NAtOncePerLength:
                while (requiredChainLength[tool] <= chainLength)
                {
                    availableToolUses[tool] += requiredChainLength[tool];
                    ++requiredChainLength[tool];
                }
                break;
            case RewardStrategy.PermanentUnlock:
                availableToolUses[tool] = -1;
                break;
            default:
                ++availableToolUses[tool];
                ++awardedToolsForCurrentChainLength[tool];

                int availableToolUsesForChainLength = GetAvailableToolUsesForChainLength(requiredChainLength[tool]);
                if (availableToolUsesForChainLength >= 0 && awardedToolsForCurrentChainLength[tool] >= availableToolUsesForChainLength)
                {
                    ++requiredChainLength[tool];
                    awardedToolsForCurrentChainLength[tool] = 0;
                }
                break;
            }
        }
    }
}
