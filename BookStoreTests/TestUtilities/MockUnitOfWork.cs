using BookStoreDataAccess.Repository;
using BookStoreDataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreTests.TestUtilities
{
    public class MockUnitOfWork
    {
        private IUnitOfWork unitOfWork;

        public IUnitOfWork GetUnitOfWork()
        {
            var dbContext = new MockDbContext().GetApplicationDbContext();

            unitOfWork = new UnitOfWork(dbContext);

            return unitOfWork;
        }
    }
}
