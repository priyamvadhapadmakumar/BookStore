using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreTests.TestUtilities
{
    public class TestData
    {
        public Book GetSampleBook()
        {
            Book book = new Book()
            {
                Author = "Test Author",
                Category = new Category()
                {
                    Id = 6,
                    Name = "Fiction"
                },
                CoverType = new CoverType()
                {
                    Id = 7,
                    Name = "Hard Cover"
                },
                Description = "Best book ever",
                Id = 12345,
                ISBN = "123ABC456DEF",
                Price = 25.99,
                Title = "The Book You Want to Buy"
            };

            return book;
        }

        public Category GetSampleCategory()
        {
            Category category = new Category()
            {
                Id = 6,
                Name = "Fiction"
            };

            return category;
        }

        public CoverType GetSampleCoverType()
        {
            CoverType coverType = new CoverType()
            {
                Id = 7,
                Name = "Hard Cover"
            };

            return coverType;
        }
    }
}
