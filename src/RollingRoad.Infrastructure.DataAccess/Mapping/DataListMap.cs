using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess.Mapping
{
    public class DataListMap : EntityMap<DataList>
    {
        public DataListMap()
        {
            Property(x => x.Name)
                .IsRequired();
            Property(x => x.Unit)
                .IsRequired();
            HasMany(x => x.Data).WithRequired(x => x.DataList);

            ToTable("DataLists");
        }
    }
}
