namespace BidSystem.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Offer
    {
        private ICollection<Bid> bids;

        public Offer()
        {
            this.bids = new HashSet<Bid>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [ForeignKey("Seller")]
        public string SellerId { get; set; }

        public virtual User Seller { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DatePublished { get; set; }

        [Required]
        public decimal InitialPrice { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }
    }
}
