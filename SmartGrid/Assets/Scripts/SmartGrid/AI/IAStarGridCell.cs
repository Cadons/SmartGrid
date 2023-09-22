namespace SmartGrid
{
    public interface IAStarGridCell : ISmartCell
    {
        public float G { get; set; }
        public float H { get; set; }
        public float F { get; }
        public IAStarGridCell Previous { get; set; }
    }
}