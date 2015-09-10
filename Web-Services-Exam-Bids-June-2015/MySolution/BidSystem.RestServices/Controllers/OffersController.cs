namespace BidSystem.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Contracts;
    using BidSystem.Data.Models;
    using BidSystem.RestServices.Infrastructure;
    using BidSystem.RestServices.Models.Offers;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/offers")]
    public class OffersController : BaseApiController
    {
        public OffersController()
            : base(new BidSystemData(new BidSystemDbContext()), new AspNetUserIdProvider())
        {
        }

        public OffersController(IBidSystemData data, IUserIdProvider userIdProvider)
            : base(data, userIdProvider)
        { 
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult ListAllOffers()
        {
            var offers = this.Data.Offers.All()
                .OrderByDescending(o => o.DatePublished)
                .Select(ListAllOffersViewModel.Create);

            return this.Ok(offers);
        }

        [HttpGet]
        [Route("active")]
        public IHttpActionResult ListAllActiveOffers()
        {
            var offers = this.Data.Offers.All()
                .OrderBy(o => o.DatePublished)
                .Where(o => DateTime.Now < o.ExpirationDate)
                .Select(ListAllOffersViewModel.Create);

            return this.Ok(offers);
        }

        [HttpGet]
        [Route("expired")]
        public IHttpActionResult ListAllExpiredOffers()
        {
            var offers = this.Data.Offers.All()
                .OrderBy(o => o.DatePublished)
                .Where(o => DateTime.Now > o.ExpirationDate)
                .Select(ListAllOffersViewModel.Create);

            return this.Ok(offers);
        }

        [HttpGet]
        [Route("details/{id}")]
        public IHttpActionResult GetOfferDetailsById(int id)
        {
            var offer = this.Data.Offers.All()
                .Where(o => o.Id == id)
                .Select(GetOfferByDetailsViewModel.Create)
                .FirstOrDefault();

            if (offer == null)
            {
                return this.NotFound();
            }

            return this.Ok(offer);
        }

        [HttpGet]
        [Authorize]
        [Route("my")]
        public IHttpActionResult ListAllUsersOffers()
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

            var offers = this.Data.Offers.All()
                .Where(o => o.SellerId == loggedUserId)
                .OrderByDescending(o => o.DatePublished)
                .Select(ListAllOffersViewModel.Create);

            return this.Ok(offers);
        }

        [Authorize]
        [HttpPost]
        public IHttpActionResult PostOffer(PostOfferBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Missing offer data.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var sellerId = this.User.Identity.GetUserId();
            var seller = this.Data.Users.All().FirstOrDefault(u => u.Id == sellerId);
            if (seller == null)
            {
                return this.Unauthorized();
            }

            var newOffer = new Offer
            {
                Title = model.Title,
                Description = model.Description,
                DatePublished = DateTime.Now,
                InitialPrice = model.InitialPrice,
                ExpirationDate = model.ExpirationDateTime,
                SellerId = sellerId
            };

            this.Data.Offers.Add(newOffer);
            this.Data.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new
                {
                    Controller = "offers/details",
                    Id = newOffer.Id
                },
                new
                {
                    Id = newOffer.Id, 
                    Seller = newOffer.Seller.UserName, 
                    Message = "Offer created."
                });
        }
    }
}
