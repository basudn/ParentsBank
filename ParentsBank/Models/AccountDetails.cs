using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ParentsBank.CustomValidations;

namespace ParentsBank.Models
{
    public class AccountDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [CustomValidation(typeof(CustomFieldValidations), "ValidateValidEmail")]
        public string Owner { get; set; }
        [Required]
        [StringLength(50)]
        [CustomValidation(typeof(CustomFieldValidations), "ValidateValidEmail")]
        public string Recipient { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OpenDate { get; set; }
        [Range(0.0,100.0)]
        public double InterestRate { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishlistItem> WishlistItems { get; set; }

        public AccountDetails()
        {
            WishlistItems = new List<WishlistItem>();
            Transactions = new List<Transaction>();
        }

        public double CalculateInterestEarnedInCurrentYear()
        {
            return 0.0;
        }
        
    }
}