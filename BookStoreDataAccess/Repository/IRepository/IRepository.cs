using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface IRepository<T> where T :class //always IRepository is generic
    {
        T Get(int id); //eg: getting record based on id, we retrieve a record from any table(eg: category)
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null, //using Linq.Expressions; for including clauses like 'where'
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, //using system.Linq
            string includeProperties = null //used for Eager loading like FK references, etc.
            ); //List of entities based on no. of requirements
        T GetFirstOrDefault(//eg: getting record based on other parameters apart from id.
            Expression<Func<T, bool>> filter = null, 
            string includeProperties = null 
            );
        void Add(T entity);
        void Remove(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);


    }
}
