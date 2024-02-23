using System.Collections;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly StoreContext _storeContext;
    //private Dictionary<string, IGenericRepository<BaseEntity>> _repositories;
    private Hashtable _repositories;  //I used this instead of a prop for every repo to achieve the open closed principle

    public UnitOfWork( StoreContext storeContext) //Ask CLR To create object from DB Context Implecitly
    {
        _storeContext = storeContext;
        _repositories = new Hashtable();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity  //This Method to create repository per request
    {
        var key = typeof(TEntity).Name;

        if(!_repositories.ContainsKey(key))
        {
            var repository = new GenericRepository<TEntity>(_storeContext) ;
            _repositories.Add(key, repository);
        }

        return _repositories[key] as IGenericRepository<TEntity>;
    }
    public async Task<int> CompleteAsync()
        => await _storeContext.SaveChangesAsync();

    public async ValueTask DisposeAsync()
    => await _storeContext.DisposeAsync();



}
