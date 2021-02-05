using BookStoreDataAccess.Data;
using BookStoreModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreTests.TestUtilities
{
    public class MockDbContext
    {
        private Book book = new Book();
        private Category category = new Category();
        private CoverType coverType = new CoverType();
        
        DbSet<Book> Books { get; set; }
        DbSet<CoverType> CoverTypes { get; set; }
        DbSet<Category> Categories { get; set; }
        
        private ApplicationDbContext dbContext;

        private void CreateDbSet()
        {
            TestData testData = new TestData();

            book = testData.GetSampleBook();
            category = testData.GetSampleCategory();
            coverType = testData.GetSampleCoverType();
        }
        public ApplicationDbContext GetApplicationDbContext()
        {
            CreateDbSet();

            Books?.Add(book);
            Categories?.Add(category);
            CoverTypes?.Add(coverType);

            var dbOptions = new DbContextOptions<ApplicationDbContext>();

            dbContext = new ApplicationDbContext(dbOptions)
            {
                Books = Books,
                Categories = Categories,
                CoverTypes = CoverTypes
            };

            return dbContext;
        }
    }
}
