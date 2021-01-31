using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CoverType coverType)
        {
            var objFromDb = _db.CoverTypes.FirstOrDefault(s =>s.Id==coverType.Id);
            /*Linq to retreive firstOrDefault. We use 's' as generic entity and for that entity we are
             * doing Id should match Category.Id. So this retrieves only one record with the condition. */
            if(objFromDb != null)
            {
                objFromDb.Name = coverType.Name;
            }
            
        }
    }
}
