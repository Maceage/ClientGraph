using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientGraph.Domain;

namespace ClientGraph.Services.Interfaces
{
    public interface IEntityService<T>
        where T : EntityBase
    {
        Task<IList<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid entityId);
        Task<bool> SaveAsync(T entity);
        Task<bool> DeleteAsync(Guid entityId);
    }
}