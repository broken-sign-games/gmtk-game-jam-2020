using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Simulation
    {
        public List<SimulationStep> Steps { get; }
        public List<Tile> ExtraneousPredictions { get; }
        public int ClearedRows { get; }

        public Simulation(List<SimulationStep> steps, List<Tile> extraneousPredictions, int clearedRows)
        {
            Steps = steps;
            ExtraneousPredictions = extraneousPredictions;
            ClearedRows = clearedRows;
        }
    } 
}