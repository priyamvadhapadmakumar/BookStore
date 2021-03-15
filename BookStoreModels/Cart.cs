using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreModels
{
    public class Cart
    {
        public Cart()
        {
            Count = 1;
        }
        [Key]
        public int Id { get; set; }
        public string AppUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [Range(1,1000,ErrorMessage ="Please enter a value between 1 and 1000")]
        public int Count  { get; set; }
        [NotMapped]
        public double Price { get; set; }

    }
}
