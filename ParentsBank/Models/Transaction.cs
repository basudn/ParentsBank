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
        [Display(Name = "Transaction Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [CustomValidation(typeof(CustomFieldValidations),"ValidateTransactionDate")]
        public DateTime TransactionDate { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateNotZero")]
        public double Amount { get; set; }
        [Required]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
    }
}