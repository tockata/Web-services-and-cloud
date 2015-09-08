﻿namespace OnlineShop.Services.Controllers
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;

    using OnlineShop.Models;
    using OnlineShop.Services.Models;

    [Authorize]
    public class AdsController : BaseApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetAds()
        {
            var ads = this.Data.Ads
                .Where(a => a.Status == AdStatus.Open)
                .OrderByDescending(a => a.Type.Name)
                .ThenBy(a => a.PostedOn)
                .Select(AdViewModel.Create);

            return this.Ok(ads);
        }

        [HttpPost]
        public IHttpActionResult CreateAd(CreateAdBindingModel model)
        {
            string userId = this.User.Identity.GetUserId();
            if (userId == null)
            {
                return this.Unauthorized();
            }

            if (model == null)
            {
                return this.BadRequest("Ad model cannot be null.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newAd = new Ad
            {
                Name = model.Name,
                Description = model.Description,
                TypeId = model.TypeId,
                Price = model.Price,
                PostedOn = DateTime.Now,
                OwnerId = userId
            };

            foreach (var categoryId in model.Categories)
            {
                var category = this.Data.Categories.Find(categoryId);
                newAd.Categories.Add(category);
            }

            this.Data.Ads.Add(newAd);
            this.Data.SaveChanges();
            var adInDb = this.Data.Ads
                .Where(a => a.Id == newAd.Id)
                .Select(AdViewModel.Create)
                .FirstOrDefault();

            return this.Ok(adInDb);
        }

        [HttpPut]
        [Route("api/ads/{id}/close")]
        public IHttpActionResult CloseAd(int id)
        {
            var ad = this.Data.Ads.Find(id);
            if (ad == null)
            {
                return this.BadRequest("There is no such ad.");
            }

            string userId = this.User.Identity.GetUserId();
            if (ad.OwnerId != userId)
            {
                return this.Unauthorized();
            }

            ad.Status = AdStatus.Closed;
            ad.ClosedOn = DateTime.Now;
            this.Data.Ads.AddOrUpdate(ad);
            this.Data.SaveChanges();

            return this.Ok();
        }
    }
}
