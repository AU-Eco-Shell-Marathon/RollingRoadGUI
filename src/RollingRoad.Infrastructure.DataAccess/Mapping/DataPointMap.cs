using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess.Mapping
{
    public class DataPointMap : EntityMap<DataPoint>
    {
        public DataPointMap()
        {
            Property(x => x.Value)
                .IsRequired();

            Property(x => x.Index)
                .IsRequired();

            ToTable("DataPoints");
        }
    }
}
