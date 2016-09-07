using ParentsBank.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Wishlist_Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int AccountId { get; set; }
        public virtual string Account { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateAdded { get; set; }
        public double Cost { get; set; }
        [Required]
        public string Description { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateValidURL")]
        public string Link { get; set; }
        public bool Purchased { get; set; }
    }
}