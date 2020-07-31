using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Simulation
    {
        public List<SimulationStep> Steps { get; }
        public ClearBoardStep ClearBoardStep { get; }
        public bool FurtherMatchesPossible { get; }
        public Tile[,] FinalGrid { get; }

        public Simulation(List<SimulationStep> steps, ClearBoardStep clearBoardStep, bool furtherMatchesPossible, Tile[,] finalGrid)
        {
            Steps = steps;
            ClearBoardStep = clearBoardStep;
            FurtherMatchesPossible = furtherMatchesPossible;
            FinalGrid = finalGrid;
        }
    } 
}