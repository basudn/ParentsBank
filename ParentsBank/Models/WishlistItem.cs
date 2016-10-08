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
        [Display(Name = "Date Added")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateAdded { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
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

        public double CalculateDifference()
        {
            return Account.Balance + Account.CalculateInterestEarnedInCurrentYear() - Cost;
        }
    }
}