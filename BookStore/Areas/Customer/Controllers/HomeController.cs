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
            //var claimsIdentity = (ClaimsIdentity)User.Identity; //To get user's identity with userid.
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //if(claim != null) //null if user hasn't logged in
            //{
            //    var count = _unitOfWork.ShoppingCart
            //        .GetAll(c => c.ApplicationUserId == claim.Value)
            //        .ToList()
            //        .Count();

            //    HttpContext.Session.SetInt32(StaticDetails.Session_Cart, count);
            //} /*after ensuring we update the session in our index page, 
            //   * we need to configure logIn razor page too. */

            return View(bookList);
        }
        public IActionResult Details(int id)
        {
            var bookFromDb = _unitOfWork.Book.
                GetFirstOrDefault(u => u.BookId == id);
            //creating a cart object to use it to add to shoppingCart just in case
            ShoppingCart cartObj = new ShoppingCart()
            {
                Book = bookFromDb,
                BookId = bookFromDb.BookId
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //can add to cart only if logged in. So only register users can order books.
        public IActionResult Details(ShoppingCart cart)
        {
            cart.Id = 0;
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

                /*booksFromCartDb - list of specific book irrespective of user. 
                 * Used to calculate total count of specific book already in cart.
                 * From this total count of specific book in cart, we can calculate
                 * number of books left in inventory
                 */
                IEnumerable<ShoppingCart> booksFromCartDb = _unitOfWork.ShoppingCart.GetAll(
                    b => b.BookId == cart.BookId, includeProperties: "Book");
                //CHECK IF INVENTORY HAS REQUESTED NO. OF BOOKS
                var books = booksFromCartDb.ToArray();
                foreach (var book in books)
                {
                    cartFromDb.SumCount += book.Count; //Calculates total count of specific book in cart irrespective of user
                }

                Inventory inventoryBook = _unitOfWork.Inventory.GetFirstOrDefault(u => u.BookId == cart.BookId);
                if ((cartFromDb.SumCount + cart.Count) > inventoryBook.Count)
                {
                    cartFromDb.ErrorMessage = $"Specified Count exceeds count in our inventory." +
                                            $"Only {inventoryBook.Count} books left!" +
                                            $" Please order within the available range/ " +
                                            $" item cannot be added to cart!";
                    return RedirectToAction(nameof(Index)); /**********TRY DIRECTING TO A SEPERATE ERROR VIEW PAGE****/
                }
                else
                {
                    if (cartFromDb == null) /*if no record exists in cart for that particular user for that particular book,
                                             * then we add the object to the cart*/
                    {
                        inventoryBook.Count -= cart.Count; //updating inventory with each cart order request
                        _unitOfWork.Inventory.Update(inventoryBook);

                        _unitOfWork.ShoppingCart.Add(cart);
                    }
                    else
                    {
                        /*if records exist in ShoppingCart db for that user for the chosen book, 
                         * add the given count to cartfromdb count and update cart db.*/
                        cartFromDb.Count += cart.Count; /*adding new cart obj count 
                                                     * to dbCart obj count so it gets added properly*/

                        inventoryBook.Count -= cart.Count; //updating inventory
                        _unitOfWork.Inventory.Update(inventoryBook);

                        /* even if we remove the below cart.update line, entity framework will track the items in db and 
                         * update automatically if we just increase the count */
                        _unitOfWork.ShoppingCart.Update(cartFromDb);
                    }

                    _unitOfWork.Save();

                    var count = _unitOfWork.ShoppingCart
                        .GetAll(c => c.ApplicationUserId == cart.ApplicationUserId)
                        .ToList()
                        .Count();

                    // HttpContext.Session.SetInt32(StaticDetails.Session_Cart, count);
                    //var sessionCartObject = HttpContext.Session.GetObject<Cart>(StaticDetails.Session_Cart);

                    return RedirectToAction(nameof(Index));
                }                
            }
            else
            {
                var bookFromDb = _unitOfWork.Book.
                    GetFirstOrDefault(u => u.BookId == cart.BookId);
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Book = bookFromDb,
                    BookId = bookFromDb.BookId
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
