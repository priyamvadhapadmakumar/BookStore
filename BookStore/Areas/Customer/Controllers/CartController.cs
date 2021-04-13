using BookStoreDataAccess.Repository.IRepository;
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

            ShoppingCartVM = new ShoppingCartViewModel()
            { 
                CartList = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==claim.Value, includeProperties:"Book")
            };
            //To populate ListCart
            foreach(var list in ShoppingCartVM.CartList)
            {
                list.Price = list.Book.Price;
                list.Book.Description = StaticDetails.ConvertToRawHtml(list.Book.Description);
                if (list.Book.Description.Length>100)
                {
                    list.Book.Description = list.Book.Description.Substring(0, 99) + "...";
                }
            }

            return View();
        }
    }
}
