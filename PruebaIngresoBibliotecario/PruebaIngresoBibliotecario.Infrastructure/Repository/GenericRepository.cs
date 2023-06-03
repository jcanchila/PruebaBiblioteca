using Microsoft.EntityFrameworkCore;
using PruebaIngresoBibliotecario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Infrastructure.Repository
{
    [ExcludeFromCodeCoverage]
    public sealed class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly PersistenceContext _context;        

        /// <summary>
        /// Constructor
        /// </summary>
        public GenericRepository(PersistenceContext context)
        {
            _context = context;                     
        }
        public async Task<bool> DeleteAsync(T entity)
        {
            IQueryable<T> query = _context.Set<T>();
            var item = query.FirstOrDefault(x => x == entity);
            if(item != null)
            {
                _context.Remove(item);
                await _context.CommitAsync().ConfigureAwait(false);
                return true;
            }

            return true;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> condition = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if(condition == null)
            {
                return await query.ToListAsync();
            }

            query = query.Where(condition);
            return await query.ToListAsync();
        }

        public async Task<T> PostAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.CommitAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task<T> PutAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.CommitAsync().ConfigureAwait(false);
            return entity;
        }
    }
}
