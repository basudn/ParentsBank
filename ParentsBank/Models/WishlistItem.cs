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
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateAdded { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateItemCost")]
        public double Cost { get; set; }
        [Required]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [StringLength(100)]
        [DataType(DataType.Url)]
        public string Link { get; set; }
        public bool Purchased { get; set; }
    }
}