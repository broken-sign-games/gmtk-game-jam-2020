using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Simulation
    {
        public List<SimulationStep> Steps { get; }
        public List<Tile> ExtraneousPredictions { get; }
        public ClearBoardStep ClearBoardStep { get; }

        public Simulation(List<SimulationStep> steps, List<Tile> extraneousPredictions, ClearBoardStep clearBoardStep)
        {
            Steps = steps;
            ExtraneousPredictions = extraneousPredictions;
            ClearBoardStep = clearBoardStep;
        }
    } 
}