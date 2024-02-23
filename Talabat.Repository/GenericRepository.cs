using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if(typeof(T) == typeof(Product))
            //    return (IEnumerable<T>)await _dbContext.Set<Product>().Include(e=>e.Brand).Include(e=>e.Category).ToListAsync();    
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            //if (typeof(T) == typeof(Product))
            //    return await _dbContext.Set<Product>().Where(e=>e.Id==id).Include(e => e.Brand).Include(e => e.Category).FirstOrDefaultAsync() as T;
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specs)
        {
            return await ApplySpecifications(specs).ToListAsync();

        }

        public async Task<T?> GetByIdWithSpecAsync(ISpecifications<T> specs)
        {
            return await ApplySpecifications(specs).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ISpecifications<T> specs)
        {
            return await ApplySpecifications(specs).CountAsync();
        }

        private IQueryable<T> ApplySpecifications(ISpecifications<T> specs)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), specs);
        }

        public async Task AddAsync(T entity)
       => await _dbContext.AddAsync(entity);

        public void Update(T entity)
        => _dbContext.Update(entity);

        public void Delete(T entity)
        => _dbContext.Remove(entity);
    }
}
