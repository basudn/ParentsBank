using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Wishlist_Item
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string Account { get; set; }
        public DateTime DateAdded { get; set; }
        public int Cost { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}