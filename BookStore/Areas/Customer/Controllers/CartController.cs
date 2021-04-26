using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")] //w/o this area will be not mapped and view won't be displayed
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; //To get user id and verify user
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //ShoppingCart cartObj = new ShoppingCart();
            //IEnumerable<ShoppingCart> cartList = _unitOfWork.ShoppingCart
            //                                    .GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Book");
            //cartObj.ApplicationUserId = claim.Value;
            //cartObj.OrderTotal = 0;

            ShoppingCartViewModel ShoppingCartVM = new ShoppingCartViewModel()
            {
                CartObject = new ShoppingCart(),
                CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Book")
                //include properties - to show all books in cart object. Foreign key reference
            };           
            ShoppingCartVM.CartObject.OrderTotal = 0;            

            //To populate ListCart
            foreach (var bookItem in ShoppingCartVM.CartList)
            {
                Inventory inventoryBook = _unitOfWork.Inventory.GetFirstOrDefault(b => b.BookId == bookItem.BookId);
                if(inventoryBook.Count > 0)
                {
                    bookItem.Message = "";
                }
                else
                {
                    bookItem.Message = "Inventory empty! No more item can be added!";
                }
                bookItem.Price = bookItem.Book.Price;
                ShoppingCartVM.CartObject.OrderTotal += (bookItem.Price * bookItem.Count);
                bookItem.Book.Description = StaticDetails.ConvertToRawHtml(bookItem.Book.Description);
                if (bookItem.Book.Description.Length > 100)
                {
                    //displaying just 1st 100 characters of description on cart view
                    bookItem.Book.Description = bookItem.Book.Description.Substring(0, 99) + "...";
                }
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCart
                .GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Book");
            Inventory inventoryItem = _unitOfWork.Inventory
                .GetFirstOrDefault(c => c.BookId == cartItem.BookId); //get count of book in inventory
            if(inventoryItem.Count > 0)
            {
                cartItem.Count += 1;
                inventoryItem.Count -= 1; //remove book from inventory
                _unitOfWork.Save();
            }
            else
            {
                //item not added
                return RedirectToAction(nameof(Index)); //validation done inside that and message displayed
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCart
                .GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Book");
            Inventory inventorybook = _unitOfWork.Inventory
                .GetFirstOrDefault(c => c.BookId == cartItem.BookId); //get count of book in inventory

            if(cartItem.Count == 1) //Last item in cart -- equivalent to removing item
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == cartItem.ApplicationUserId)
                    .ToList()
                    .Count; //gets count of cart objects for that particular user
                _unitOfWork.ShoppingCart.Remove(cartItem); //since, we have only 1 item in cart, we remove cart item
                inventorybook.Count += 1; //add the book to inventory
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.Session_Cart, (count - 1)); //decreasing session count
            }
            else
            {
                cartItem.Count -= 1;
                inventorybook.Count += 1;
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCart
                .GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Book");
            Inventory inventoryBook = _unitOfWork.Inventory
                .GetFirstOrDefault(c => c.BookId == cartItem.BookId); //get count of book in inventory
            
            var count = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == cartItem.ApplicationUserId)
                .ToList()
                .Count; //gets count of cart objects for that particular user
            _unitOfWork.ShoppingCart.Remove(cartItem); //since, we have only 1 item in cart, we remove cart item
            //inventoryCount += 1; //add the book to inventory
            inventoryBook.Count += cartItem.Count;
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(StaticDetails.Session_Cart, (count - 1)); //decreasing session count
            
            return RedirectToAction(nameof(Index));
        }

    }
}
