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
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Owner { get; set; }
        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Recipient { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OpenDate { get; set; }
        [CustomValidation(typeof(CustomFieldValidations),"ValidateInterestRate")]
        public double InterestRate { get; set; }
        public double Balance { get; set; }
        public double BeginBalance { get; set; }
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