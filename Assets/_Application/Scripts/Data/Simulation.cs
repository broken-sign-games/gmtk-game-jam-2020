using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Simulation
    {
        public List<SimulationStep> Steps { get; }

        public Simulation(List<SimulationStep> steps)
        {
            Steps = steps;
        }
    } 
}