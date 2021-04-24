using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreModels.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart CartObject { get; set; }
        public IEnumerable<ShoppingCart> CartList { get; set; } //to display list of items in cart

    }
}
