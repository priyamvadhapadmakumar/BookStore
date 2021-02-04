using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreUtility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")] //must add this
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View(); //after this, create corresponding view
        }
        public IActionResult Upsert(int? id)
        {/* id can be null if creating a new coverType and
          * id has a value if editing an existing coverType*/
            CoverType coverType = new CoverType();
            if(id == null) //For creating new coverType
            {
                return View(coverType);
            }
            //for edit
            var parameter = new DynamicParameters();//using Dapper
            parameter.Add("@id", id);
            coverType = _unitOfWork.StoredProcedureCall.OneRecord<CoverType>(
                StaticDetails.ProcGetCoverType,parameter);
            
            if(coverType == null)
            {
                return NotFound();
            }
            return View(coverType);//view coverType that was retreived from db for the given id
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /*writes a unique value to a http only cookie and same value is returned to the form. After the
         * page is submitted, an error is raised when this value doesn't match with the form value. 
         * This prevents cross site request forgery (i.e) a form from another site that posts to your site 
         * in an attempt to submit hidden content using authenticated user's credentials.
         */
        public IActionResult Upsert(CoverType coverType)
        {
            if(ModelState.IsValid)//CHECKS ALL VALIDATIONS ARE CHECKED IN GET METHOD AND CLIENT SIDE
                //double security feature
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);//to create, in our stored procedure, we give name as parameter
                if(coverType.Id==0) 
                {
                    _unitOfWork.StoredProcedureCall.Execute(StaticDetails.ProcCreateCoverType,parameter);
                    
                }
                else //to update, we update using id
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.StoredProcedureCall.Execute(StaticDetails.ProcUpdateCoverType,parameter);
                    
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);//if validations not true, gives back original form to check inputs
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() //used in coverType.js(created after creating index view)
        {
            var allObj = _unitOfWork.StoredProcedureCall.List<CoverType>(
                StaticDetails.ProcGetAllCoverType,null);//add static details(like all procedure names in utilities)
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();//using Dapper
            parameter.Add("@id", id);
            var objFromDb = _unitOfWork.StoredProcedureCall.OneRecord<CoverType>(
                StaticDetails.ProcGetCoverType, parameter);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.StoredProcedureCall.Execute(StaticDetails.ProcDeleteCoverType, parameter);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion
    }
}
