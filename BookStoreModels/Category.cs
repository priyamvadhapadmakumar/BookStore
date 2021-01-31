using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStoreModels
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Category Name")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }/*after creating the table, 'add-migration'. After migration file created, we notice that it will be
      * empty. This is because we need to make some changes in AplicationDbContext.cs file*/
}
