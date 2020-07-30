using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Simulation
    {
        public List<SimulationStep> Steps { get; }
        public ClearBoardStep ClearBoardStep { get; }

        public Simulation(List<SimulationStep> steps, ClearBoardStep clearBoardStep)
        {
            Steps = steps;
            ClearBoardStep = clearBoardStep;
        }
    } 
}