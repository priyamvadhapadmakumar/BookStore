using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookStoreModels
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public string EbayPrice { get; set; } //For comparison purposes
        [NotMapped]
        public string FoundStatus { get; set; }
    }
}
