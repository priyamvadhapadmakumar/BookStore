using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Book = new BookRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new CartRepository(_db);
            StoredProcedureCall = new StoredProcedureCall(_db);
        }

        public IBookRepository Book { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICartRepository ShoppingCart { get; private set; }
        public IStoredProcedureCall StoredProcedureCall { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
        
        /*in our common repository (Repository.cs), all the changes made by each of the methods are not
         * explicitly saved inside respective methods. They just perform the changes. 
         * So to save all other changes to our db, we have this Save method in this UnitOfWork*/
        public void Save()
        {
            _db.SaveChanges();
        }

        /*after creating this UnitOfWork, we need make all these available to the project by adding them
         * to the services in our StartUp.cs file*/
    }
}
