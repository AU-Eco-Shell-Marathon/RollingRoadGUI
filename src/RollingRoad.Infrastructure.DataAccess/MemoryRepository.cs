using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RollingRoad.Core.DomainServices;

namespace RollingRoad.Infrastructure.DataAccess
{
    public class MemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly IList<T> _entities = new List<T>();

        public IQueryable<T> AsQueryable()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public T Create()
        {
            throw new NotImplementedException();
        }

        public void DeleteByKey(params object[] key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int? page = default(int?), int? pageSize = default(int?))
        {
            return _entities.AsEnumerable();
        }

        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int? page = default(int?), int? pageSize = default(int?))
        {
            throw new NotImplementedException();
        }

        public T GetByKey(params object[] key)
        {
            throw new NotImplementedException();
        }

        public T Insert(T entity)
        {
            _entities.Add(entity);
            return entity;
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
