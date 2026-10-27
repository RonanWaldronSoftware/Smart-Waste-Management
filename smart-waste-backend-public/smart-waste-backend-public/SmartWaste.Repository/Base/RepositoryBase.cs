using SmartWaste.Model.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;

namespace SmartWaste.Repository.Base
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext) => RepositoryContext = repositoryContext;
        
        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ?
            RepositoryContext.Set<T>()
            .AsNoTracking() :
            RepositoryContext.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
            !trackChanges ?
            RepositoryContext.Set<T>()
            .Where(expression)
            .AsNoTracking() :
            RepositoryContext.Set<T>()
            .Where(expression);

        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
        public void CreateList(List<T> entityList) => RepositoryContext.Set<T>().AddRange(entityList);
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
        public void UpdateList(List<T> entityList) => RepositoryContext.Set<T>().UpdateRange(entityList);
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
        public void DeleteList(List<T> entityList) => RepositoryContext.Set<T>().RemoveRange(entityList);
        public async Task<bool> Any(Expression<Func<T, bool>> expression) => await RepositoryContext.Set<T>().AnyAsync(expression);

    }
}
