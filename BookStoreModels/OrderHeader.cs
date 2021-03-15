using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreModels
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string AppUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public DateTime ShippingDate { get; set; }
        [Required]
        public double OrderTotal { get; set; }
        public string Carrier { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentDueDate { get; set; }//for cash on delivery
        public string TransactionId { get; set; }
        //Personal info
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
    }
}
