﻿using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")] //must add this
    [Authorize(Roles = StaticDetails.Role_Admin +","+ StaticDetails.Role_Employee)]
    public class UserController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork; - repository pattern  
        private readonly ApplicationDbContext _appDb; //connection through applicationDb
        /*Usually it is bad practice to combine both technologies in a project.
         * However, for self-learning and practice, doing both in this*/
        public UserController(ApplicationDbContext appDb)
        {
            _appDb = appDb;
        }
        public IActionResult Index()
        {
            return View(); 
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() //used in category.js(created after creating index view)
        {
            var userList = _appDb.ApplicationUsers.Include(u => u.Company).ToList();
            /*includes if the user is a company user based on companyId foreign key*/
            var userRole = _appDb.UserRoles.ToList();/*UserRoles table created by default by aspnetcore
                                                      * It has all users mapped to corresponding roles*/
            var roles = _appDb.Roles.ToList(); //Lists all roles from Roles table in db.
            foreach(var user in userList) //To list users their corresponding roles
            {
                var roleId = userRole.FirstOrDefault(userRole => userRole.UserId == user.Id).RoleId;
                /*from userRole table, we check  if the user from the userList(variable sent inside loop)
                 * is present by checking the the user id in usertable and user id in userRoles table
                 * If there's a match it gets the corresponding Roleid from the userRoles table or if not, a default value for id 
                 * is set to roleId variable*/
                user.Role = roles.FirstOrDefault(user => user.Id == roleId).Name; 
                /*the unmapped property - Role : Sis not saved as column of user table in db 
                 * but can be assigned value inside the app by assigning the name of roleId 
                 * mapped for corresponding users in previous step*/
                if(user.Company==null)
                {
                    user.Company = new Company
                    {
                        Name = "" //or it will throw null reference error as this is a required field
                    };
                }

            }
            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            /*Apply the [FromBody] attribute to a parameter to populate 
             * its properties from the body of an HTTP request - REFER below site
             * https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-5.0
             */
            var objFromDb = _appDb.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking!" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //User is currently locked, so we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(500);
            }
            _appDb.SaveChanges();
            return Json(new { success = true, message = "Action Successful!" });
        }
        #endregion
    }
}
