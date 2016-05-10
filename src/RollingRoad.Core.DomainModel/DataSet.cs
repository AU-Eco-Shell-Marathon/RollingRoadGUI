using System.Collections.Generic;

namespace RollingRoad.Core.DomainModel
{
    public class DataSet : IEntity
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description written in the file
        /// </summary>
        public string Description { get; set; }
        public virtual ICollection<DataList> DataLists { get; set; } = new List<DataList>();

        public override string ToString()
        {
            return Name;
        }

        public int Id { get; set; }
    }
}
