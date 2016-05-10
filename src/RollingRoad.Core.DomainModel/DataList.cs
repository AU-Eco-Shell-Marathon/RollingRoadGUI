using System.Collections.Generic;

namespace RollingRoad.Core.DomainModel
{
    public class DataList : IEntity
    {
        public int Id { get; set; }
        public DataSet DataSet { get; set; }

        public string Name { get; set; }
        public string Unit { get; set; }
        
        public virtual ICollection<DataPoint> Data { get; set; } = new List<DataPoint>();

        public DataList()
        {
            Name = "Unknown";
            Unit = "Unknown";
        }

        /// <summary>
        /// Creates a new data list with the specified name and unit
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="System.ArgumentException">Thrown when name or unit is empty or null</exception>
        public DataList(string name, string unit)
        {
            Name = name;
            Unit = unit;
        }

        public override string ToString()
        {
            return $"{Name} ({Unit}). ({Data.Count} datapoints)";
        }
    }
}
