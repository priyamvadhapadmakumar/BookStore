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
        IStoredProcedureCall StoredProcedureCall { get; }
        void Save();
    }
}
