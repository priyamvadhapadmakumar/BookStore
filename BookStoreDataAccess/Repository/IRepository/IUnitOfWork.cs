using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Book { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ICartRepository ShoppingCart { get; }
        IStoredProcedureCall StoredProcedureCall { get; }
        void Save();
    }
}
