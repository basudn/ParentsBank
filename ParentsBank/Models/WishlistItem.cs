using ParentsBank.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class WishlistItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual int AccountId { get; set; }
        public virtual AccountDetails Account { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateAdded { get; set; }

        //Cost must be provided
        public double Cost { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        //Description mus be provided
        [StringLength(100)]
        [CustomValidation(typeof(CustomFieldValidations), "ValidateValidURL")]
        public string Link { get; set; }
        public bool Purchased { get; set; }
    }
}