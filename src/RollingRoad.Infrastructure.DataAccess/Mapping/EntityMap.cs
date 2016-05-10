using System.Data.Entity.ModelConfiguration;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Infrastructure.DataAccess.Mapping
{
    public abstract class EntityMap<T> : EntityTypeConfiguration<T> where T : class, IEntity
    {
        protected EntityMap()
        {
            HasKey(t => t.Id);
        }
    }
}
