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
            if (value == 0)
            {
                return new ValidationResult("Transaction amount cannot be 0.");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateInterestRate(double value, ValidationContext context)
        {
            if (value <= 0)
            {
                return new ValidationResult("Interest rate should be greater than 0.");
            } else if (value >= 100)
            {
                return new ValidationResult("Interest rate should be less than 100.");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateItemCost(double value, ValidationContext context)
        {
            if (value <= 0)
            {
                return new ValidationResult("Item cost must be provided.");
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateTransactionDate(DateTime transactionDate, ValidationContext context)
        {
            if(transactionDate > DateTime.Now)
            {
                return new ValidationResult("Transaction date cannot be in future.");
            } else if(transactionDate.Year < DateTime.Now.Year)
            {
                return new ValidationResult("Transaction date cannot be before the current year.");
            }
            return ValidationResult.Success;
        }
    }
}