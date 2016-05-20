using System;
using System.Collections.Generic;
using ClientGraph.Domain;

namespace ClientGraph.Services.Interfaces
{
    public interface IEntityService<T>
        where T : EntityBase
    {
        IList<T> GetAll();
        T GetById(Guid entityId);
        bool Save(T entity);
        bool Delete(Guid entityId);
    }
}