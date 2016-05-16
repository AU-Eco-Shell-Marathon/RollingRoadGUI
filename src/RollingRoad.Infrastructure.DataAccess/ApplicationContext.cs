using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using RollingRoad.Core.DomainModel;
using RollingRoad.Infrastructure.DataAccess.Mapping;

namespace RollingRoad.Infrastructure.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class ApplicationContext : DbContext
    {

        public ApplicationContext() : base("Name=ApplicationContext")
        {

        }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }


        public IDbSet<DataSet> DataSets { get; set; }
        public IDbSet<DataList> DataLists { get; set; }
        public IDbSet<DataPoint> DataPoint { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DataSetMap());
            modelBuilder.Configurations.Add(new DataListMap());
            modelBuilder.Configurations.Add(new DataPointMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
