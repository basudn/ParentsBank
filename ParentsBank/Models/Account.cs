using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ParentsBank.CustomValidations;

namespace ParentsBank.Models
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateValidEmail")]
        public string Owver { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateValidEmail")]
        public string Recipient { get; set; }
        public string Name { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OpenDate { get; set; }
        [CustomValidation(typeof(CustomFieldValidations), "ValidateInterestRate")]
        public double InterestRate { get; set; }

        public double CalculateInterestEarnedInCurrentYear()
        {
            return 0.0;
        }
    }
}