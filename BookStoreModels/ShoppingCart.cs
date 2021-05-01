using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreModels
{
    public class ShoppingCart
    {  
        public ShoppingCart()
        {
            Count = 1;
            //SumCount = 0;
        }
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [Range(1, 100, ErrorMessage = "Please enter a value between 1 and 100")] 
        //Randomly set - we accept orders only with max.100 books in a single "Add to cart" button click
        public int Count { get; set; }
        //[NotMapped]
        //public int SumCount { get; set; } /*To get count of same books ordered by different users 
        //                                   * to make sure that a new user doesn't order the same
        //                                   * book with count more than that is available in our inventory*/
        [NotMapped]
        public double Price { get; set; }

        [NotMapped]
        public double OrderTotal { get; set; }
        [NotMapped]
        public string InventoryMessage { get;set; }
        [NotMapped]
        public string CartMessage { get; set; }

    }
}
