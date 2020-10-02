using System.Text;

namespace GMTK2020.Data
{
    public class Level
    {
        public Board Board { get; }
        public Simulation Simulation { get; }
        public Level(Board board, Simulation simulation)
        {
            Board = board;
            Simulation = simulation;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            foreach (int y in Board.GetYs(VerticalOrder.TopToBottom))
            {
                foreach (int x in Board.GetXs(HorizontalOrder.LeftToRight))
                {
                    message.Append($"{Board[x, y]?.Color.ToString() ?? "_"}\t");
                }
                message.AppendLine("");
            }
            return message.ToString();
        }
    }
}