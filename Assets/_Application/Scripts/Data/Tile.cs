namespace GMTK2020.Data
{
    public sealed class Tile
    {
        public int Color { get; }

        public bool Inert { get; private set; }
        public bool Marked { get; set; }

        public Tile(int color)
        {
            Color = color;
        }

        public void MakeInert()
        {
            Inert = true;
            Marked = false;
        }
    }
}
