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
        public virtual string Account { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TransactionDate { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateNotZero")]
        public double Amount { get; set; }
        [Required]
        public string Note { get; set; }
    }
}