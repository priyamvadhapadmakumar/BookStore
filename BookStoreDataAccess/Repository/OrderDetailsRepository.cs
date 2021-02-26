using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails orderDetails)
        {
            _db.Update(orderDetails);
        }
    }
}
