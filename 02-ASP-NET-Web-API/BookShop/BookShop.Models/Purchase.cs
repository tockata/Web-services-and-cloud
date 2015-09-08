namespace BookShopSystem.Models
{
    using System;

    public class Purchase
    {
        public Purchase()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; }

        public bool IsRecalled { get; set; }

        public Guid BookId { get; set; }

        public virtual Book Book { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
