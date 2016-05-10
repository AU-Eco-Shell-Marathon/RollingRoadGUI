using System.Threading.Tasks;

namespace RollingRoad.Core.DomainServices
{
    public interface IUnitOfWork
    {
        int Save();
        Task<int> SaveAsync();
    }
}
