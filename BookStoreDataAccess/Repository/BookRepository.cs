using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Book book)
        {
            var objFromDb = _db.Books.FirstOrDefault(s =>s.BookId==book.BookId);
            /*Linq to retreive firstOrDefault. We use 's' as generic entity and for that entity we are
             * doing Id should match Book.Id. So this retrieves only one record with the condition. */
            if(objFromDb != null)
            {
                if(objFromDb.ImageUrl != null)
                {
                    objFromDb.ImageUrl = book.ImageUrl;
                }

                objFromDb.ISBN = book.ISBN;
                objFromDb.Title = book.Title;
                objFromDb.Author = book.Author;
                objFromDb.Description = book.Description;
                objFromDb.Price = book.Price;

            }
            
        }
    }
}
