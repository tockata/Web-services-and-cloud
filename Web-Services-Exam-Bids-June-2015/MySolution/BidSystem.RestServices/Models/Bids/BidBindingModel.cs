namespace BidSystem.RestServices.Models.Bids
{
    using System.ComponentModel.DataAnnotations;

    public class BidBindingModel
    {
        [Required]
        public decimal BidPrice { get; set; }

        public string Comment { get; set; }
    }
}