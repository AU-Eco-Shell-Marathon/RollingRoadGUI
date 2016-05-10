using RollingRoad.Core.DomainModel;

namespace RollingRoad.Core.ApplicationServices
{
    public interface IDataSetLoader
    {
        DataSet LoadFromFile(string path);
    }
}
