using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using BookStoreUtility;
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
            //cartObj.OrderTotal = 0;

            ShoppingCartViewModel ShoppingCartVM = new ShoppingCartViewModel()
            {
                CartObject = new ShoppingCart(),
                CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Book")
                //include properties - to show all books in cart object. Foreign key reference
            };           
            ShoppingCartVM.CartObject.OrderTotal = 0;

            //To populate ListCart
            foreach (var list in ShoppingCartVM.CartList)
            {
                list.Price = list.Book.Price;
                ShoppingCartVM.CartObject.OrderTotal += (list.Price * list.Count);
                list.Book.Description = StaticDetails.ConvertToRawHtml(list.Book.Description);
                if (list.Book.Description.Length > 100)
                {
                    //displaying just 1st 100 characters of description on cart view
                    list.Book.Description = list.Book.Description.Substring(0, 99) + "...";
                }
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Book");
            var inventoryCount = _unitOfWork.Inventory.GetFirstOrDefault(c => c.BookId == cart.BookId).Count; //get count of book in inventory
            if(inventoryCount > 0)
            {
                cart.Count += 1;
                inventoryCount -= 1;
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
                //******************Try to display error message on Index view
            }
        }
    }
}
