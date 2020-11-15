using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020
{
    public class Toolbox
    {
        private Simulator simulator;
        private ToolDataMap toolData;

        private Dictionary<Tool, int> availableToolUses;
        private Dictionary<MatchShape, Tool> shapeRewards;

        public Toolbox(ToolDataMap toolData, Simulator simulator)
        {
            this.toolData = toolData;
            this.simulator = simulator;

            availableToolUses = new Dictionary<Tool, int>();
            shapeRewards = new Dictionary<MatchShape, Tool>();

            foreach ((Tool tool, ToolDataMap.ToolData data) in toolData.Map)
            {
                availableToolUses[tool] = data.InitialUses;

                if (data.AwardedFor != MatchShape.None)
                {
                    if (shapeRewards.ContainsKey(data.AwardedFor))
                        throw new ArgumentOutOfRangeException(nameof(toolData), "Multiple tools associated with the same shape");

                    shapeRewards[data.AwardedFor] = tool;
                }
            }
        }

        public int GetAvailableUses(Tool tool)
            => availableToolUses[tool];

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

            return step;
        }

        public void RewardMatches(MatchStep step)
        {
            foreach (Vector2Int match in step.LeftEndsOfHorizontalMatches)
            {
                RewardShape(MatchShape.Row3);

                if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(1, 0)))
                    RewardShape(MatchShape.Row4);

                if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(2, 0)))
                    RewardShape(MatchShape.Row5);

                if (step.BottomEndsOfVerticalMatches.Contains(match))
                {
                    RewardShape(MatchShape.L);

                    if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(0, 2)))
                    {
                        RewardShape(MatchShape.U);

                        if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, 0)))
                            RewardShape(MatchShape.O);
                    }

                    if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, 0)))
                        RewardShape(MatchShape.U);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, 0)))
                {
                    RewardShape(MatchShape.L);

                    if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(0, 2)))
                        RewardShape(MatchShape.U);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, -2)))
                {
                    RewardShape(MatchShape.L);

                    if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -2)))
                        RewardShape(MatchShape.U);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -2)))
                    RewardShape(MatchShape.L);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(1, 0)))
                {
                    RewardShape(MatchShape.T);

                    if (step.LeftEndsOfHorizontalMatches.Contains(match + new Vector2Int(0, 2)))
                        RewardShape(MatchShape.H);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(1, -2)))
                    RewardShape(MatchShape.T);


                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, -1)))
                {
                    RewardShape(MatchShape.T);

                    if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -1)))
                        RewardShape(MatchShape.H);
                }

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(2, -1)))
                    RewardShape(MatchShape.T);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(1, -1)))
                    RewardShape(MatchShape.Plus);
            }

            foreach (Vector2Int match in step.BottomEndsOfVerticalMatches)
            {
                RewardShape(MatchShape.Row3);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, 1)))
                    RewardShape(MatchShape.Row4);

                if (step.BottomEndsOfVerticalMatches.Contains(match + new Vector2Int(0, 2)))
                    RewardShape(MatchShape.Row5);
            }
        }

        private void RewardShape(MatchShape shape)
        {
            if (!shapeRewards.ContainsKey(shape))
                return;

            Tool tool = shapeRewards[shape];

            if (availableToolUses[tool] >= 0)
                ++availableToolUses[tool];
        }
    }
}
