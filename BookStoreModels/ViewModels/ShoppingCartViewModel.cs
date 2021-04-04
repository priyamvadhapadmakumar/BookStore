using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreModels.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> CartList { get; set; } //to display list of items in cart
        public OrderHeader OrderHeader { get; set; } //To get the user type(spplication user)

    }
}
