namespace GMTK2020.Data
{
    public enum RotationSense
    {
        CW,
        CCW,
    } 

    public static class RotationSenseExtensions
    {
        public static RotationSense Other(this RotationSense rotSense)
            => rotSense == RotationSense.CW ? RotationSense.CCW : RotationSense.CW;
    }
}