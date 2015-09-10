namespace BidSystem.RestServices.Models.Offers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BidSystem.Data.Models;
    using BidSystem.RestServices.Models.Bids;

    public class GetOfferByDetailsViewModel
    {
        public static Expression<Func<Offer, GetOfferByDetailsViewModel>> Create
        {
            get
            {
                return o => new GetOfferByDetailsViewModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.DatePublished,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = DateTime.Now > o.ExpirationDate,
                    BidsCount = o.Bids.Count,
                    BidWinner = o.Bids.Count > 0 && DateTime.Now > o.ExpirationDate ?
                        o.Bids.OrderByDescending(b => b.BidPrice).Select(b => b.Bidder.UserName).FirstOrDefault()
                        : null,
                    Bids = o.Bids.OrderByDescending(b => b.Date).Select(b => new BidViewModel
                    {
                        Id = b.Id,
                        OfferId = b.OfferId,
                        OfferedPrice = b.BidPrice,
                        Bidder = b.Bidder.UserName,
                        DateCreated = b.Date,
                        Comment = b.Comment
                    })
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Seller { get; set; }

        public DateTime DatePublished { get; set; }

        public decimal InitialPrice { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public bool IsExpired { get; set; }

        public int BidsCount { get; set; }

        public string BidWinner { get; set; }

        public IEnumerable<BidViewModel> Bids { get; set; }
    }
}