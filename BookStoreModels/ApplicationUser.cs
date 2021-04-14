using Microsoft.AspNetCore.Identity; //for inheriting basic identity provided by asp.netcore
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookStoreModels
{
    public enum Role
    {
        Administrator,
        IndependentCustomer
    }
    public class ApplicationUser:IdentityUser
    { /*this adds some properties to already existing dbo.aspnetusers table in db 
       * which is created by default when creating this app*/
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        [NotMapped] //this property not pushed to DB
        public string Role { get; set; }        
    }
}
