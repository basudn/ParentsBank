using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ParentsBank.CustomValidations
{
    public class CustomFieldValidations
    {
        public static ValidationResult ValidateNotZero(double value, ValidationContext context)
        {
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateInterestRate(double value, ValidationContext context)
        {
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateValidURL(string URL, ValidationContext context)
        {
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateValidEmail(string email, ValidationContext context)
        {
            return ValidationResult.Success;
        }
    }
}