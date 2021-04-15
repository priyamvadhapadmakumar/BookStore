using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser appUser)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(s => s.Id == appUser.Id);
            var userRoleIdFromDb = _db.UserRoles.FirstOrDefault(u => u.UserId == appUser.Id).RoleId;
            var userRoleFromDb = _db.Roles.FirstOrDefault(r => r.Id == userRoleIdFromDb).Name;
            if(objFromDb != null)
            {
                objFromDb.Name = appUser.Name;
                objFromDb.Address = appUser.Address;
                objFromDb.City = appUser.City;
                objFromDb.State = appUser.State;
                objFromDb.PostalCode = appUser.PostalCode;
                objFromDb.PhoneNumber = appUser.PhoneNumber;
                userRoleFromDb = appUser.Role; 
            }
        }
    }
}
