namespace BidSystem.RestServices.Models.Offers
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PostOfferBindingModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal InitialPrice { get; set; }

        [Required]
        public DateTime ExpirationDateTime { get; set; }
    }
}