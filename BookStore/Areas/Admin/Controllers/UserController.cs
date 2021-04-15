using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreUtility;
using Dapper;
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
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; //- repository pattern  
        private readonly ApplicationDbContext _appDb; //connection through applicationDb
        /*Usually it is bad practice to combine both technologies in a project.
         * However, for self-learning and practice, doing both in this*/
        public UserController(IUnitOfWork unitOfWork,ApplicationDbContext appDb)
        {
            _unitOfWork = unitOfWork;
            _appDb = appDb;
        }
        public IActionResult Index()
        {
            return View(); 
        }
        public IActionResult Edit(string id)
        {
            ApplicationUser appUser = _unitOfWork.ApplicationUser.Get(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }
        //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/updating-related-data-with-the-entity-framework-in-an-asp-net-mvc-application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.ApplicationUser.Update(user);
                    _unitOfWork.Save();
                }
                catch
                {
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() 
        {
            var userList = _appDb.ApplicationUsers.ToList();
            var userRole = _appDb.UserRoles.ToList();/*UserRoles table created by default by aspnetcore
                                                      * It has all users mapped to corresponding roles*/
            var roles = _appDb.Roles.ToList(); //Lists all roles from Roles table in db.
            foreach(var user in userList) //To list users their corresponding roles
                //application user model created by us does not map roles
                //To do this using UserRoles table provided by ASP.NET, we have this foreach loop
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                /*from userRole table, we check  if the user from the userList(variable sent inside loop)
                 * is present by checking the the user id in usertable and user id in userRoles table
                 * If there's a match it gets the corresponding Roleid from the userRoles table or if not, a default value for id 
                 * is set to roleId variable*/
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name; 
                /*the unmapped property - Role : Sis not saved as column of user table in db 
                 * but can be assigned value inside the app by assigning the name of roleId 
                 * mapped for corresponding users in previous step*/

            }
            return Json(new { data = userList });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var objFromDb = _appDb.ApplicationUsers.Find(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _appDb.ApplicationUsers.Remove(objFromDb);
            _appDb.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });

        }
        //[HttpPost]
        //public IActionResult LockUnlock([FromBody] string id)
        //{
        //    /*Apply the [FromBody] attribute to a parameter to populate 
        //     * its properties from the body of an HTTP request - REFER below site
        //     * https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-5.0
        //     */
        //    var objFromDb = _appDb.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        //    if (objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "Error while Locking/Unlocking!" });
        //    }
        //    if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
        //    {
        //        //User is currently locked, so we will unlock them
        //        objFromDb.LockoutEnd = DateTime.Now;
        //    }
        //    else
        //    {
        //        objFromDb.LockoutEnd = DateTime.Now.AddYears(500);
        //    }
        //    _appDb.SaveChanges();
        //    return Json(new { success = true, message = "Action Successful!" });
        //}
        #endregion
    }
}
