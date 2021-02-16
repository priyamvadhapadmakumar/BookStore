using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View(); 
        }
        public IActionResult Upsert(int? id)
        { 
            Company company = new Company();
            if(id == null) //For creating new company
            {
                return View(company);
            }
            //for edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault()); ;
            
            if(company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)//CHECKS ALL VALIDATIONS ARE CHECKED IN GET METHOD AND CLIENT SIDE
                //double security feature
            {
                if(company.Id==0)
                {
                    _unitOfWork.Company.Add(company);
                    
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);//if validations not true, gives back original form to check inputs
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() //used in company.js(created after creating index view)
        {
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion
    }
}
