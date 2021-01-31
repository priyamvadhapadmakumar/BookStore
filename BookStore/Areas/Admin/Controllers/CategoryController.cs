using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")] //must add this
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View(); //after this, create corresponding view
        }
        public IActionResult Upsert(int? id)
        {/* id can be null if creating a new category and
          * id has a value if editing an existing category*/
            Category category = new Category();
            if(id == null) //For creating new category
            {
                return View(category);
            }
            //for edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault()); ;
            //id.GetValueOrDefault and not .Get because the given id can be null/ something that isn't on db
            if(category == null)
            {
                return NotFound();
            }
            return View(category);//view category that was retreived from db for the given id
        }

        [HttpPost("upsert-category")]
        [ValidateAntiForgeryToken]
        /*writes a unique value to a http only cookie and same value is returned to the form. After the
         * page is submitted, an error is raised when this value doesn't match with the form value. 
         * This prevents cross site request forgery (i.e) a form from another site that posts to your site 
         * in an attempt to submit hidden content using authenticated user's credentials.
         */
        public IActionResult Upsert(Category category)
        {
            if(ModelState.IsValid)//CHECKS ALL VALIDATIONS ARE CHECKED IN GET METHOD AND CLIENT SIDE
                //double security feature
            {
                if(category.Id==0)
                {
                    _unitOfWork.Category.Add(category);
                    
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                    
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);//if validations not true, gives back original form to check inputs
        }

        #region API CALLS

        [HttpGet("get-all-categories")]
        public IActionResult GetAll() //used in category.js(created after creating index view)
        {
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete("delete-category")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion
    }
}
