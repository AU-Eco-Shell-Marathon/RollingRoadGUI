using RollingRoad.Core.DomainModel;

namespace RollingRoad.Core.ApplicationServices
{
    public interface IDataSetDataFile
    {
        void WriteFile(string path, DataSet dataset, string header);
        DataSet LoadFromFile(string path);
    }
}
