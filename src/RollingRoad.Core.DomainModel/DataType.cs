namespace RollingRoad.Core.DomainModel
{
    public class DataType
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public DataType()
        {
            
        }

        public DataType(string name, string unit)
        {
            Name = name;
            Unit = unit;
        }
    }
}
