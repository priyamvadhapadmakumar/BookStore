﻿using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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

            /*all the below lines of code makes sure that if a user logs out and
             * logs back in, the user is able to retrieve the session he left.
             * Like, the cart still has the books he added before logging out.*/
            var claimsIdentity = (ClaimsIdentity)User.Identity; //To get user's identity with userid.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null) //null if user hasn't logged in
            {
                var count = _unitOfWork.Cart
                    .GetAll(c => c.AppUserId == claim.Value)
                    .ToList()
                    .Count();

                HttpContext.Session.SetInt32(StaticDetails.Session_Cart, count);
            } /*after ensuring we update the session in our index page, 
               * we need to configure logIn razor page too. */

            return View(bookList);
        }
        public IActionResult Details(int id)
        {
            var bookFromDb = _unitOfWork.Book.
                GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            Cart cartObj = new Cart()
            {
                Book = bookFromDb,
                BookId = bookFromDb.Id
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //can add to cart only if logged in. So only register users can order books.
        public IActionResult Details(Cart cart)
        {
            cart.Id = 0;
            if(ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.AppUserId = claim.Value;

                Cart cartFromDb = _unitOfWork.Cart.GetFirstOrDefault(
                    u => u.AppUserId == cart.AppUserId && u.BookId == cart.BookId,
                    includeProperties: "Book");

                if(cartFromDb == null)
                {
                    //no records exist in database for that book for that user. So add the cart to unit of work.
                    _unitOfWork.Cart.Add(cart);
                }
                else
                {
                    //if records exist in database, add cart count to cartfromdb count and update unit of work with cart.
                    cartFromDb.Count += cart.Count;

                    /* even if we remove the below cart.update line, entity framework will track the items in db and 
                     * update automatically if we just increase the count */
                    _unitOfWork.Cart.Update(cartFromDb);
                }

                _unitOfWork.Save();

                var count = _unitOfWork.Cart
                    .GetAll(c => c.AppUserId == cart.AppUserId)
                    .ToList()
                    .Count();

                HttpContext.Session.SetInt32(StaticDetails.Session_Cart, count);
                //var sessionCartObject = HttpContext.Session.GetObject<Cart>(StaticDetails.Session_Cart);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var bookFromDb = _unitOfWork.Book.
                    GetFirstOrDefault(u => u.Id == cart.BookId, includeProperties: "Category,CoverType");
                Cart cartObj = new Cart()
                {
                    Book = bookFromDb,
                    BookId = bookFromDb.Id
                };
                return View(cartObj);
            }
            
            
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
