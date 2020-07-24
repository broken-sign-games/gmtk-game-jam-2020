namespace GMTK2020.Data
{
    public sealed class Tile
    {
        public int Color { get; private set; }

        public Tile(int color)
        {
            Color = color;
        }

        public void Petrify()
            => Color = -1;
    }
}
