using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
