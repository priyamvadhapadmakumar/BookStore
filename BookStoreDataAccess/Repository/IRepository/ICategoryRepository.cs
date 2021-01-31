using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
