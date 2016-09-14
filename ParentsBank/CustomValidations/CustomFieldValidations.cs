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
            if (value == 0.0)
            {
                return new ValidationResult("Amount cannot be zero");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateValidURL(string URL, ValidationContext context)
        {
            if(URL.StartsWith("www.")&&URL.EndsWith(".com"))
            {
                return new ValidationResult("Enter valid URL");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateValidEmail(string email, ValidationContext context)
        {
            if (email.Contains("@")&&email.EndsWith(".com")&&!email.StartsWith("@"))
            {
                return new ValidationResult("Enter valid URL");
            }
            return ValidationResult.Success;
        }
    }
}