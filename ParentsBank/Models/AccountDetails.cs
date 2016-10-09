using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ParentsBank.CustomValidations;
using System.Data;
using System.Data.Entity;
using System.Linq;

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
        [Display(Name = "Open Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OpenDate { get; set; }
        [Display(Name = "Interest Rate")]
        [CustomValidation(typeof(CustomFieldValidations), "ValidateInterestRate")]
        public double InterestRate { get; set; }
        public double Balance { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishlistItem> WishlistItems { get; set; }

        public AccountDetails()
        {
            WishlistItems = new List<WishlistItem>();
            Transactions = new List<Transaction>();
        }

        public string GetLastTransactionDate()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Transaction> transactions = db.Transactions.Where(t => t.Account.Id == Id && t.Amount >= 0).OrderByDescending(t => t.TransactionDate).ToList();
            if(transactions.Count == 0)
            {
                return "";
            } else
            {
                return transactions[0].TransactionDate.ToString("MM/dd/yyyy");
            }
        }

        public double CalculateInterestEarnedInCurrentYear()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
            var lastDay = new DateTime(DateTime.Now.Year, 12, 31);
            var yearlyDaysInt = lastDay.DayOfYear;
            double runningTotal = 0.00000;
            double yearlyAmount = 0.0;
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var date1 = firstDay;
            var date2 = firstDay;
            DateTime previous = firstDay;
            List<Transaction> listTrans = db.Transactions.Where(t => t.Account.Id == Id && t.TransactionDate >= firstDay).ToList();
            while (date2 <= today)
            {
                foreach (var it_trans in listTrans)
                {
                    if (it_trans.TransactionDate.Equals(date2))
                    {
                        yearlyAmount += it_trans.Amount;
                    }
                }
                date2 = date2.AddDays(1);
            }

            if (Balance > yearlyAmount)
            {
                runningTotal = Balance - yearlyAmount;
            }
            while (date1 <= today)
            {
                foreach (var it_trans in listTrans)
                {
                    if (it_trans.TransactionDate.Equals(date1))
                    {
                        runningTotal += it_trans.Amount;
                    }
                }
                runningTotal = runningTotal * Math.Pow(1.0 + (InterestRate / (100 * 12)), (12.0 * 1 / yearlyDaysInt));
                date1 = date1.AddDays(1);
            }
            return Math.Round(runningTotal - yearlyAmount, 2);
        }
    }
}
