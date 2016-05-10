namespace RollingRoad.Core.DomainModel
{
    public class DataPoint : IEntity
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public double Value { get; set; }
        public DataList DataList { get; set; }

        public DataPoint()
        {
            Index = -1;
            Value = 0;
        }

        public DataPoint(double value)
        {
            Value = value;
        }
    }
}
