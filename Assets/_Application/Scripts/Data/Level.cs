using System.Text;

namespace GMTK2020.Data
{
    public class Level
    {
        public Tile[,] Grid { get; }
        public Simulation Simulation { get; }
        public Level(Tile[,] grid, Simulation simulation)
        {
            Grid = grid;
            Simulation = simulation;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            for (int y = Grid.GetLength(1) - 1; y >= 0; --y)
            {
                for (int x = 0; x < Grid.GetLength(0); ++x)
                {
                    message.Append($"{Grid[x, y]?.Color ?? 0}\t");
                }
                message.AppendLine("");
            }
            return message.ToString();
        }
    }
}