using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using BookStoreTests.TestUtilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreTests
{
    [TestClass]
    public class BookTests
    {
        private ApplicationDbContext dbContext = new MockDbContext().GetApplicationDbContext();
        private TestData testData = new TestData();

        [TestMethod]
        public void UpdateBookRepository_Updates()
        {
            var bookRepository = new BookRepository(dbContext);

            bookRepository.Update(dbContext.Books.FirstOrDefault());
        }

        [TestMethod]
        public void BookUpsert_Works()
        {
            List<SelectListItem> categoryList = new List<SelectListItem>();
            List<SelectListItem> coverTypeList = new List<SelectListItem>();
            SelectListItem selectItem = new SelectListItem()
            {
                Selected = true,
                Value = "default"
            };

            categoryList.Add(selectItem);
            coverTypeList.Add(selectItem);

            BookVM bookView = new BookVM()
            {
                Book = testData.GetSampleBook(),
                CategoryList = categoryList,
                CoverTypeList = coverTypeList
            };
        }
    }
}
