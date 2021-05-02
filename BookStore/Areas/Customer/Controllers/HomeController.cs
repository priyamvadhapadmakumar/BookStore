using BookStore.MyWebScraper;
using BookStoreDataAccess.Repository.IRepository;
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
            IEnumerable<Book> bookList = _unitOfWork.Book.GetAll();

            /*all the below lines of code makes sure that if a user logs out and
             * logs back in, the user is able to retrieve the session he left.
             * Like, the cart still has the books he added before logging out.*/

            var claimsIdentity = (ClaimsIdentity)User.Identity; //To get user's identity with userid.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null) //null if user hasn't logged in
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList()
                    .Count();

                HttpContext.Session.SetInt32(StaticDetails.Session_Cart, count);
            } /*after ensuring we update the session in our index page, 
            //   * we need to configure logIn razor page too. */

            return View(bookList);
        }
        public IActionResult Details(int id)
        {
            string message;
            int count = 0;
            var bookFromDb = _unitOfWork.Book.
                GetFirstOrDefault(u => u.BookId == id);

            var soldBooks = _unitOfWork.ShoppingCart.GetAll(b => b.BookId == id).ToList();
            var soldBooksArray = soldBooks.ToArray();
            foreach(ShoppingCart book in soldBooksArray)
            {
                count += book.Count;
            }

            var inventoryBook = _unitOfWork.Inventory.GetFirstOrDefault(i => i.BookId == id); 
            if(inventoryBook.Count == 0)
            {
                message = "All sold!";
            }
            else
            {
                message = $"Only {inventoryBook.Count} left!";
            }
            //creating a cart object to use it to add to shoppingCart just in case
            ShoppingCart cartObj = new ShoppingCart()
            {
                Book = bookFromDb,
                BookId = bookFromDb.BookId,
                InventoryMessage = message,
                CartMessage = $"{count} books sold!"
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //can add to cart only if logged in. So only register users can order books.
        public IActionResult Details(ShoppingCart cart)
        {
            cart.Id = 0; //initial - not logged in scenario
            if(ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value; //claim.Vlaue property stores userId of the IdentityUser

                //cartObjFromDb - obj of cart of specific user and specific book
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cart.ApplicationUserId && u.BookId == cart.BookId,
                    includeProperties: "Book");

                Inventory inventoryBook = _unitOfWork.Inventory.GetFirstOrDefault(u => u.BookId == cart.BookId,includeProperties:"Book");

                if(cartFromDb == null) //if user adds this book for the 1st time to his cart --prevents null reference error
                {
                    //CHECK IF INVENTORY HAS REQUESTED NO. OF BOOKS 
                    if (inventoryBook.Count >= cart.Count) //inventory Has books
                    {
                        inventoryBook.Count -= cart.Count; //updating inventory with each cart order request
                        _unitOfWork.Inventory.Update(inventoryBook);

                        _unitOfWork.ShoppingCart.Add(cart);
                    }
                    else //inventory doesn't have requested no.of books to be added to cart
                    {
                        ShoppingCart cartObj = new ShoppingCart()
                        {
                            Book = inventoryBook.Book,
                            BookId = inventoryBook.BookId,
                            InventoryMessage = $"Please check your order count. Only {inventoryBook.Count} left!"
                        };
                        return View(cartObj);
                    }

                }
                else //if user already has this book in his cart
                {
                    //CHECK IF INVENTORY HAS REQUESTED NO. OF BOOKS 
                    if (cart.Count >= inventoryBook.Count)
                    {
                        ShoppingCart cartObj = new ShoppingCart()
                        {
                            Book = inventoryBook.Book,
                            BookId = inventoryBook.BookId,
                            InventoryMessage = $"Please check your order count. Only {inventoryBook.Count} left!"
                        };
                        return View(cartObj);
                    }
                    else //inventory has books
                    {
                        inventoryBook.Count -= cart.Count;
                        cartFromDb.Count += cart.Count;
                    }
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == cart.ApplicationUserId)
                    .ToList()
                    .Count();

                HttpContext.Session.SetInt32(StaticDetails.Session_Cart, count);
                //SetInt32 - default otpion provided by ASP.NET CORE

                /*OR -- CUSTOM SESSION IMPLEMENTATION -
                    * If we want to store anything apart from integer value in session, 
                    * below code - cos GetObject & SetObject methods we created in sessionExtension are Generic types
                    */
                //var sessionCartObject = HttpContext.Session.GetObject<Cart>(StaticDetails.Session_Cart);

                return RedirectToAction(nameof(Index));               
            }
            else
            {
                var bookFromDb = _unitOfWork.Book.
                    GetFirstOrDefault(u => u.BookId == cart.BookId);
                Inventory inventoryBook = _unitOfWork.Inventory
                    .GetFirstOrDefault(u => u.BookId == cart.BookId);
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Book = bookFromDb,
                    BookId = bookFromDb.BookId,
                    InventoryMessage = $"Only {inventoryBook.Count} left!"
                };
                return View(cartObj);
            }    
        }

        public IActionResult Compare(int id)
        {
            var bookFromDb = _unitOfWork.Book.GetFirstOrDefault(u => u.BookId == id);

            WebScraper webScraper = new WebScraper();

            bookFromDb.AmazonPrice = webScraper.GetPrice(bookFromDb.ISBN).ToString();

            if(bookFromDb.AmazonPrice.Equals("0"))
            {
                bookFromDb.FoundStatus = "Book not found on Amazon for comparison!";
            }
            else
            {
                bookFromDb.FoundStatus = "Book found on Amazon!";
            }
            return View(bookFromDb); 
        }

       
        public IActionResult TopSellers()
        {
            IEnumerable<BooksSold> items = _unitOfWork.StoredProcedureCall.List<BooksSold>(StaticDetails.Proc_BestSellingBooks); 
            if(items == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var itemsList = items.ToList();
            foreach( var item in itemsList)
            {
                item.ImageUrl = _unitOfWork.Book.GetFirstOrDefault(b => b.Title == item.Title).ImageUrl;
                item.BookId = _unitOfWork.Book.GetFirstOrDefault(i => i.Title == item.Title).BookId;
            }
            return View(itemsList);
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
