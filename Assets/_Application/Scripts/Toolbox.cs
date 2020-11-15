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

        public Toolbox(ToolDataMap toolData, Simulator simulator)
        {
            this.toolData = toolData;
            this.simulator = simulator;

            availableToolUses = new Dictionary<Tool, int>();

            foreach ((Tool tool, ToolDataMap.ToolData data) in toolData.Map)
            {
                availableToolUses[tool] = data.InitialUses;
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
    }
}
