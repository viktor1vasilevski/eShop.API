using Domain.Interfaces;
using eShop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class SqlUnitOfWork<TContext> : IDisposable, IUnitOfWork<TContext> where TContext : DbContext, new()
{
    private TContext _context;
    private bool disposed = false;

    public SqlUnitOfWork(TContext context)
    {
        _context = context;
    }

    public void DetachAllEntities()
    {
        var entities = _context.ChangeTracker.Entries();
        foreach (var entry in entities)
        {
            entry.State = EntityState.Detached;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this.disposed = true;
    }

    public IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
    {
        return new SqlGenericRepository<TEntity, TContext>(_context);
    }

    public void Restart()
    {
        if (_context != null)
        {
            _context.Dispose();
        }
        _context = new TContext();
    }

    public TContext ReturnContext() => _context;

    public void RevertChanges()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    {
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    }
                case EntityState.Deleted:
                    {
                        entry.State = EntityState.Unchanged;
                        break;
                    }
                case EntityState.Added:
                    {
                        entry.State = EntityState.Detached;
                        break;
                    }
            }
        }
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
