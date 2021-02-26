using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        IBookRepository Book { get; }
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ICartRepository Cart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IStoredProcedureCall StoredProcedureCall { get; }
        void Save();
    }
}
