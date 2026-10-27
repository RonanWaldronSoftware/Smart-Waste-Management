using SmartWaste.Model.Core;
using System.Linq.Expressions;

namespace SmartWaste.Repository.Base
{
    internal interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool trackChanges);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
        void Create(T entity);
        void Update(T entity);
        void UpdateList(List<T> entityList);
        void Delete(T entity);
        void DeleteList(List<T> entityList);
        Task<bool> Any(Expression<Func<T, bool>> expression);
    }
}
