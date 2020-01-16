using System;
using System.Collections.Generic;

namespace AuthenticationService.Repositories
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAll();

        T GetById(Guid id);

        void Create(T entity); 

        void Update(T entity); 

        void Delete(Guid id); 

        void Save();  
    }
}
