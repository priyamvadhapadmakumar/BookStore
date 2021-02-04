using Microsoft.AspNetCore.Mvc.Rendering; //For SelectListItems
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreModels.ViewModels
{
    public class BookVM
    {
        public Book Book { get; set; }
        //selectListItem is rendered as HTML. It gives a list of items
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }

    }
}
