namespace BidSystem.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Contracts;
    using BidSystem.Data.Models;
    using BidSystem.RestServices.Infrastructure;
    using BidSystem.RestServices.Models.Bids;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api")]
    public class BidsController : BaseApiController
    {
        public BidsController()
            : base(new BidSystemData(new BidSystemDbContext()), new AspNetUserIdProvider())
        {
        }

        public BidsController(IBidSystemData data, IUserIdProvider userIdProvider)
            : base(data, userIdProvider)
        { 
        }

        [HttpGet]
        [Authorize]
        [Route("bids/my")]
        public IHttpActionResult ListUserBids()
        {
            var loggedUserId = this.UserIdProvider.GetUserId();
            if (loggedUserId == null)
            {
                return this.Unauthorized();
            }

            var userInDb = this.Data.Users.All()
                .FirstOrDefault(u => u.Id == loggedUserId);

            if (userInDb == null)
            {
                return this.Unauthorized();
            }

            var bids = this.Data.Bids.All()
                .Where(b => b.BidderId == loggedUserId)
                .OrderByDescending(b => b.Date)
                .Select(BidViewModel.Create);

            return this.Ok(bids);
        }

        [HttpGet]
        [Authorize]
        [Route("bids/won")]
        public IHttpActionResult ListUserWonBids()
        {
            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId == null)
            {
                return this.Unauthorized();
            }

            var userInDb = this.Data.Users.All()
                .FirstOrDefault(u => u.Id == loggedUserId);

            if (userInDb == null)
            {
                return this.Unauthorized();
            }

            var bids = this.Data.Bids.All()
                .Where(b => b.BidderId == loggedUserId && DateTime.Now > b.Offer.ExpirationDate &&
                    b.Offer.Bids.
                    OrderByDescending(bid => bid.BidPrice)
                    .Select(bid => bid.Bidder.Id)
                    .FirstOrDefault() == loggedUserId)
                .OrderBy(b => b.Date)
                .Select(BidViewModel.Create);

            return this.Ok(bids);
        }

        [HttpPost]
        [Route("offers/{id}/bid")]
        public IHttpActionResult BidForOffer(int id, BidBindingModel model)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId == null)
            {
                return this.Unauthorized();
            }

            var userInDb = this.Data.Users.All()
                .FirstOrDefault(u => u.Id == loggedUserId);
            if (userInDb == null)
            {
                return this.Unauthorized();
            }

            var offerInDb = this.Data.Offers.All()
                .FirstOrDefault(o => o.Id == id);
            if (offerInDb == null)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (DateTime.Now > offerInDb.ExpirationDate)
            {
                return this.Content(
                    HttpStatusCode.BadRequest,
                    new { Message = "Offer has expired." });
            }

            if (model.BidPrice < offerInDb.InitialPrice)
            {
                return this.Content(
                    HttpStatusCode.BadRequest,
                    new { Message = "Your bid should be > " + offerInDb.InitialPrice });
            }

            if (offerInDb.Bids.Count > 0 && model.BidPrice <= offerInDb.Bids.Max(b => b.BidPrice))
            {
                return this.Content(
                    HttpStatusCode.BadRequest, 
                    new { Message = "Your bid should be > " + offerInDb.Bids.Max(b => b.BidPrice) });
            }

            var newBid = new Bid
            {
                BidPrice = model.BidPrice,
                Comment = model.Comment,
                BidderId = loggedUserId,
                Date = DateTime.Now,
                OfferId = offerInDb.Id
            };

            this.Data.Bids.Add(newBid);
            this.Data.SaveChanges();

            return this.Ok(new
            {
                Id = newBid.Id,
                Bidder = newBid.Bidder.UserName,
                Message = "Bid created."
            });
        }
    }
}
