namespace BidSystem.RestServices.Models.Offers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using BidSystem.Data.Models;

    public class ListAllOffersViewModel
    {
        public static Expression<Func<Offer, ListAllOffersViewModel>> Create
        {
            get
            {
                return o => new ListAllOffersViewModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.DatePublished,
                    InitialPrice = o.InitialPrice,
                    ExpirationDate = o.ExpirationDate,
                    IsExpired = DateTime.Now > o.ExpirationDate,
                    BidsCount = o.Bids.Count,
                    BidWinner = o.Bids.Count > 0 && DateTime.Now > o.ExpirationDate ?
                        o.Bids.OrderByDescending(b => b.BidPrice).Select(b => b.Bidder.UserName).FirstOrDefault()
                        : null
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Seller { get; set; }

        public DateTime DatePublished { get; set; }

        public decimal InitialPrice { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsExpired { get; set; }

        public int BidsCount { get; set; }

        public string BidWinner { get; set; }
    }
}