using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T,bool>> condition = null);
        Task<T> PostAsync(T entity);
        Task<T> PutAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
