//Model just for a particular view of model. Not common for all views of model.
using Microsoft.AspNetCore.Mvc.Rendering; //For SelectListItems
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreModels.ViewModels
{
    public class BookVM
    {
        public Book Book { get; set; }
      
    }
}
