using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Customer.Controllers //rename namespace based on the folders
{
    [Area("Customer")] //explicitly define the area name before you move/create any controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        //Since nothing defined, it is a [GET] action method
        public IActionResult Index()
        {
            IEnumerable<Book> bookList = _unitOfWork.Book.GetAll(includeProperties: "Category,CoverType");
            return View(bookList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
