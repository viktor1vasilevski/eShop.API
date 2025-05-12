using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork<TContext> where TContext : DbContext, new()
    {
        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
        void Dispose();
        void SaveChanges();
        Task SaveChangesAsync();
        void RevertChanges();
        void DetachAllEntities();
        void Restart();
        TContext ReturnContext();

    }
}
