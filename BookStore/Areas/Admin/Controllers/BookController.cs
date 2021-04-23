using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;//For IWebHostEnvironment
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;//For Path.Combine for uploading images
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //To upload images in the app inside the wwwroot folder
        private readonly IWebHostEnvironment _hostEnvironment; 
        public BookController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View(); 
        }
        public IActionResult Upsert(int? id)
        {
            /*Create a viewModel under BookStoreModels folder inside ViewModels folder. This view is
             * custom for this Upsert action method*/
            Book book = new Book(); //Instance of Book view model(not Book) with its properties .
            if(id == null) //For creating new book
            {
                return View(book);
            }

            //for edit
            book = _unitOfWork.Book.Get(id.GetValueOrDefault()); 
            if(book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Book book)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if(files.Count>0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\books");
                    var extension = Path.GetExtension(files[0].FileName);

                    if(book.ImageUrl != null)
                    {
                        //edit - to remove old image
                        var imagePath = Path.Combine(webRootPath, book.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    book.ImageUrl = @"\images\books\" + fileName + extension;
                }
                else
                {
                    //edit - image not changed
                    if(book.BookId != 0)
                    {
                        Book objFromDb = _unitOfWork.Book.Get(book.BookId);
                        book.ImageUrl = objFromDb.ImageUrl;
                    }
                }//end 'if-else' for edit Book w or w/o changing image of book

                if (book.BookId == 0)
                {
                    _unitOfWork.Book.Add(book);

                } // creating new book. For image, we get through above if-else steps where we add new image.
                else
                {
                    _unitOfWork.Book.Update(book);

                }//edit existing book. Depending on availability of new image, we update or use same old image (1st if-else block)
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else //if ModelState.IsNotValid--> Means no client side validations done. This block does server side validations
            {
                if(book.BookId!=0) //only for edit 
                {
                    book = _unitOfWork.Book.Get(book.BookId);
                }
            }
            return View(book);//if validations not true, gives back original form to check inputs
        }

        /**************************************************/
        //ADD 'COMPARE' METHOD AS GET&POST METHODS / API CALLS
        /**************************************************/

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() 
        {
            var allObj = _unitOfWork.Book.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Book.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Book.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion
    }
}
