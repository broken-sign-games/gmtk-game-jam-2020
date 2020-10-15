using System.Text;

namespace GMTK2020.Data
{
    public class Level
    {
        public Board Board { get; }
        public Level(Board board)
        {
            Board = board;
        }
    }
}