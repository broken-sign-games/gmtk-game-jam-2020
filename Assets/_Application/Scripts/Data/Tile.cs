namespace GMTK2020.Data
{
    public sealed class Tile
    {
        public int Color { get; private set; }
        public bool IsStone => Color < 0;
        public bool Marked { get; set; }

        public Tile(int color)
        {
            Color = color;
        }

        public void Petrify()
            => Color = -1;
    }
}
