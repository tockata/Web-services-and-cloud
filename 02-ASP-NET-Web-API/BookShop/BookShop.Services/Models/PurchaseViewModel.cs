namespace BookShop.Services.Models
{
    using System;

    public class PurchaseViewModel
    {
        public string Buyer { get; set; }

        public string BookTitle { get; set; }
        
        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; }

        public bool IsRecalled { get; set; }
    }
}