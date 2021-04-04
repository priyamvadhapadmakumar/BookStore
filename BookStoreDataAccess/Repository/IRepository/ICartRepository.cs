using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart cart);
    }
}
