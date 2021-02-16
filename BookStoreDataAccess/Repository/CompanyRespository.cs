using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            _db.Update(company); //using entity framework to do the job of updating rather than us updating individual record.
        }
    }
}
