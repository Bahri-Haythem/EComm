using Domain.Entities;
using Domain.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);

    Task<IReadOnlyList<T>> GetListAsync(ISpecification<T> spec);

    Task<int> CountAsync(ISpecification<T> spec);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}
