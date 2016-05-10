using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess.Mapping
{
    public class DataSetMap : EntityMap<DataSet>
    {
        public DataSetMap()
        {
            Property(t => t.Name)
                .IsRequired();

            Property(t => t.Description)
                .IsRequired();

            HasMany(x => x.DataLists)
                .WithRequired(x => x.DataSet);

            ToTable("DataSet");
        }
    }
}
