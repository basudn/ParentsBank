using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string Account { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Amount { get; set; }
        public string Note { get; set; }

    }
}