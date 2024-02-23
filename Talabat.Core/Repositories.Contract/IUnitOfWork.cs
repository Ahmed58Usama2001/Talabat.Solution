using Talabat.Core.Repositories.Contract;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Repositories.Contract;

public interface IUnitOfWork:IAsyncDisposable 
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    Task<int> CompleteAsync();
}
