using ParentsBank.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual int AccountId { get; set; }


        public virtual AccountDetails Account { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]

        //A transaction date cannot be in the future and cannot be before the current year

        
        public DateTime TransactionDate { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateNotZero")]

        //A transaction cannot be for a 0.00 amount
        //The amount of the transaction is negative(indicates a debit) or positive(indicates a credit)
        //A debit cannot be for more than the current account balance
        public double Amount { get; set; }

        [Required]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
    }
}