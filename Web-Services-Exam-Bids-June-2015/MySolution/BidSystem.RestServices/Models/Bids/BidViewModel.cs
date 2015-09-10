namespace BidSystem.RestServices.Models.Bids
{
    using System;
    using System.Linq.Expressions;
    using BidSystem.Data.Models;

    public class BidViewModel
    {
        public static Expression<Func<Bid, BidViewModel>> Create
        {
            get
            {
                return b => new BidViewModel
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    OfferedPrice = b.BidPrice,
                    Bidder = b.Bidder.UserName,
                    DateCreated = b.Date,
                    Comment = b.Comment
                };
            }
        }

        public int Id { get; set; }

        public int OfferId { get; set; }

        public decimal OfferedPrice { get; set; }

        public string Bidder { get; set; }

        public DateTime DateCreated { get; set; }

        public string Comment { get; set; }
    }
}