using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookStoreModels
{
    public class BooksSold
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int books_Sold { get; set; }
        public string ImageUrl { get; set; }
    }
}
