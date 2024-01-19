using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> Complete();
}
