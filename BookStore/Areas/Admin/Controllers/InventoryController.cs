using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStoreDataAccess.Data;
using BookStoreModels;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Admin.Controllers //Created this controller with views using EntityFramework option.
{
    [Area("Admin")]
    [Authorize(Roles =BookStoreUtility.StaticDetails.Role_Admin)]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        //For practice, using both types of connections to database (Repository pattern & DbContext)

        public InventoryController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Inventories
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Inventories/Upsert
        public IActionResult Upsert(int? id)
        {
            InventoryVM inventoryVM = new InventoryVM()
            {
                Inventory = new Inventory(),
                BookList = _unitOfWork.Book.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.BookId.ToString()
                })
            };

            //Create
            if(id == null)
            {
                return View(inventoryVM);
            }

            //Edit functionality
            if(inventoryVM == null)
            {
                return NotFound();
            }
            return View(inventoryVM);
        }

        // POST: Admin/Inventories/Upsert
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(InventoryVM inventoryVM)
        {
            if (ModelState.IsValid)
            {
                if (inventoryVM.Inventory.InventoryId== 0)
                {
                    _unitOfWork.Inventory.Add(inventoryVM.Inventory);

                }
                else
                {
                    _unitOfWork.Inventory.Update(inventoryVM.Inventory);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(inventoryVM);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var allObj = _unitOfWork.Inventory.GetAll();
                return Json(new { data = allObj });
            }
            catch(NullReferenceException ex)
            {
                return Json(new { data = ex.Message });
            }
            
        }

        #endregion
    }
}
