namespace BidSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Bid
    {
        public int Id { get; set; }

        [Required]
        public int OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        [Required]
        public decimal BidPrice { get; set; }

        [Required]
        [ForeignKey("Bidder")]
        public string BidderId { get; set; }

        public virtual User Bidder { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public string Comment { get; set; }
    }
}
