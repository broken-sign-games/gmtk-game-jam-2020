using GMTK2020;
using GMTK2020.Data;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests
{
    public class ToolboxTest
    {
        private ToolDataMap toolData;

        [SetUp]
        public void InitializeTest()
        {
            toolData = ScriptableObject.CreateInstance<ToolDataMap>();
            toolData.Map[Tool.ToggleMarked].InitialUses = -1;
            toolData.Map[Tool.RemoveTile].AwardedFor = MatchShape.Row3;
            toolData.Map[Tool.RemoveRow].AwardedFor = MatchShape.Row4;
            toolData.Map[Tool.CreateWildcard].AwardedFor = MatchShape.Row5;
            toolData.Map[Tool.PlusBomb].AwardedFor = MatchShape.Plus;
            toolData.Map[Tool.RemoveColor].AwardedFor = MatchShape.T;
            toolData.Map[Tool.Rotate3x3].AwardedFor = MatchShape.L;
            toolData.Map[Tool.SwapTiles].AwardedFor = MatchShape.H;
            toolData.Map[Tool.SwapLines].AwardedFor = MatchShape.U;
        }

        [Test]
        public void Toolbox_takes_initial_uses_from_tool_data()
        {
            toolData.Map[Tool.RemoveTile].InitialUses = 3;

            Toolbox toolbox = CreateToolbox();

            Assert.That(toolbox.GetAvailableUses(Tool.ToggleMarked), Is.EqualTo(-1));
            Assert.That(toolbox.GetAvailableUses(Tool.RemoveTile), Is.EqualTo(3));
            Assert.That(toolbox.GetAvailableUses(Tool.CreateWildcard), Is.EqualTo(0));
        }

        [Test]
        public void Using_tool_reduces_available_uses()
        {
            toolData.Map[Tool.RemoveTile].InitialUses = 3;

            Toolbox toolbox = CreateToolbox();

            toolbox.UseTool(Tool.RemoveTile, Vector2Int.zero);

            Assert.That(toolbox.GetAvailableUses(Tool.RemoveTile), Is.EqualTo(2));
        }

        [Test]
        public void Negative_tool_use_allows_infinite_usage()
        {
            Toolbox toolbox = CreateToolbox();

            toolbox.UseTool(Tool.ToggleMarked, Vector2Int.zero);

            Assert.That(toolbox.GetAvailableUses(Tool.ToggleMarked), Is.EqualTo(-1));
        }

        [Test]
        public void Zero_available_uses_cause_tool_use_to_fail()
        {
            Toolbox toolbox = CreateToolbox();

            Assert.Throws<InvalidOperationException>(
                () => toolbox.UseTool(Tool.CreateWildcard, Vector2Int.zero));

            Assert.That(toolbox.GetAvailableUses(Tool.CreateWildcard), Is.EqualTo(0));
        }

        [Test]
        public void Using_swap_tool_reduces_available_uses()
        {
            toolData.Map[Tool.SwapTiles].InitialUses = 3;

            Toolbox toolbox = CreateToolbox();

            toolbox.UseSwapTool(Tool.SwapTiles, Vector2Int.zero, Vector2Int.right);

            Assert.That(toolbox.GetAvailableUses(Tool.SwapTiles), Is.EqualTo(2));
        }

        [Test]
        public void Negative_swap_tool_use_allows_infinite_usage()
        {
            toolData.Map[Tool.SwapTiles].InitialUses = -1;

            Toolbox toolbox = CreateToolbox();

            toolbox.UseSwapTool(Tool.SwapTiles, Vector2Int.zero, Vector2Int.right);

            Assert.That(toolbox.GetAvailableUses(Tool.SwapTiles), Is.EqualTo(-1));
        }

        [Test]
        public void Zero_available_uses_cause_swap_tool_use_to_fail()
        {
            Toolbox toolbox = CreateToolbox();

            Assert.Throws<InvalidOperationException>(
                () => toolbox.UseSwapTool(Tool.SwapTiles, Vector2Int.zero, Vector2Int.right));

            Assert.That(toolbox.GetAvailableUses(Tool.SwapTiles), Is.EqualTo(0));
        }

        [TestCase(Tool.SwapTiles)]
        [TestCase(Tool.SwapLines)]
        public void Swap_tools_cannot_be_used_with_UseTool(Tool swapTool)
        {
            toolData.Map[swapTool].InitialUses = 1;

            Toolbox toolbox = CreateToolbox();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => toolbox.UseTool(swapTool, Vector2Int.zero));
        }

        [TestCase(Tool.ToggleMarked)]
        [TestCase(Tool.RemoveRow)]
        [TestCase(Tool.RemoveTile)]
        [TestCase(Tool.RemoveColor)]
        [TestCase(Tool.RefillInert)]
        [TestCase(Tool.CreateWildcard)]
        [TestCase(Tool.Rotate3x3)]
        [TestCase(Tool.Bomb)]
        [TestCase(Tool.PlusBomb)]
        public void Only_swap_tools_can_be_used_with_UseSwapTool(Tool tool)
        {
            toolData.Map[tool].InitialUses = 1;

            Toolbox toolbox = CreateToolbox();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => toolbox.UseSwapTool(tool, Vector2Int.zero, Vector2Int.right));
        }

        [TestCase(Tool.ToggleMarked, typeof(PredictionStep))]
        [TestCase(Tool.RemoveRow, typeof(RemovalStep))]
        [TestCase(Tool.RemoveTile, typeof(RemovalStep))]
        [TestCase(Tool.RemoveColor, typeof(RemovalStep))]
        [TestCase(Tool.CreateWildcard, typeof(WildcardStep))]
        [TestCase(Tool.Rotate3x3, typeof(RotationStep))]
        [TestCase(Tool.Bomb, typeof(RemovalStep))]
        [TestCase(Tool.PlusBomb, typeof(RemovalStep))]
        public void UseTool_returns_simulation_step(Tool tool, Type stepType)
        {
            toolData.Map[tool].InitialUses = 1;

            Toolbox toolbox = CreateToolbox();

            SimulationStep step = toolbox.UseTool(tool, Vector2Int.one);

            Assert.That(step, Is.TypeOf(stepType));
        }

        [TestCase(Tool.SwapTiles, typeof(PermutationStep))]
        [TestCase(Tool.SwapLines, typeof(PermutationStep))]
        public void UseSwapTool_returns_simulation_step(Tool tool, Type stepType)
        {
            toolData.Map[tool].InitialUses = 1;

            Toolbox toolbox = CreateToolbox();

            SimulationStep step = toolbox.UseSwapTool(tool, Vector2Int.one, new Vector2Int(2, 1));

            Assert.That(step, Is.TypeOf(stepType));
        }

        private Toolbox CreateToolbox()
        {
            Board board = IntGridToBoard(new int[,]
            {
                { 1, 5, 9, 2, 1, 1, 2, 3, 5 },
                { 2, 6, 1, 6, 4, 3, 5, 3, 4 },
                { 1, 5, 2, 7, 2, 4, 4, 4, 9 },
                { 1, 3, 2, 2, 5, 1, 3, 3, 3 },
                { 7, 1, 3, 4, 3, 1, 2, 2, 1 },
                { 8, 4, 7, 6, 1, 1, 2, 3, 6 },
                { 2, 3, 1, 7, 9, 3, 4, 8, 7 },
                { 4, 5, 5, 6, 2, 2, 5, 4, 1 },
                { 1, 2, 3, 2, 1, 3, 2, 5, 6 },
            });

            var simulator = new Simulator(board, 9);

            return new Toolbox(toolData, simulator);
        }

        private static Board IntGridToBoard(int[,] intGrid)
        {
            int width = intGrid.GetLength(0);
            int height = intGrid.GetLength(1);
            var board = new Board(height, width);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    int color = intGrid[x, y];
                    if (color == 0)
                        continue;

                    var tile = new Tile(Math.Abs(color) - 1);
                    board[y, width - x - 1] = tile;

                    if (color < 0)
                        tile.Marked = true;
                }

            return board;
        }
    }
}
