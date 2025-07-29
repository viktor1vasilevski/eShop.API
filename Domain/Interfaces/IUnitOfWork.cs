using eShop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces;

public interface IUnitOfWork<TContext> where TContext : DbContext
{
    IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
    void Dispose();
    void SaveChanges();
    Task SaveChangesAsync();
    void RevertChanges();
    void DetachAllEntities();
    TContext ReturnContext();

}
