namespace BidSystem.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using BidSystem.RestServices.Models.Offers;
    using BidSystem.Tests.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OfferDetailsIntegrationTests
    {
        [TestMethod]
        public void GetOfferDetails_Of_Existing_Offer_Should_Return_200Ok_And_Offer_Details()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            var offersToAdds = new OfferModel[]
            {
                new OfferModel() { Title = "First Offer (Expired)", Description = "Description", InitialPrice = 200, ExpirationDateTime = DateTime.Now.AddDays(-5)},
                new OfferModel() { Title = "Another Offer (Expired)", InitialPrice = 15.50m, ExpirationDateTime = DateTime.Now.AddDays(-1)},
                new OfferModel() { Title = "Second Offer (Active 3 months)", Description = "Description", InitialPrice = 500, ExpirationDateTime = DateTime.Now.AddMonths(3)},
                new OfferModel() { Title = "Third Offer (Active 6 months)", InitialPrice = 120, ExpirationDateTime = DateTime.Now.AddMonths(6)},
            };
            foreach (var offer in offersToAdds)
            {
                var httpResult = TestingEngine.CreateOfferHttpPost(userSession.Access_Token, offer.Title, offer.Description, offer.InitialPrice, offer.ExpirationDateTime);
            }

            var firstOfferId = TestingEngine.GetRandomOfferIdFromDb();
            var getOfferDetailsHttpResult = TestingEngine.GetOfferDetailsHttpGet(firstOfferId);
            Assert.AreEqual(HttpStatusCode.OK, getOfferDetailsHttpResult.StatusCode);
            var offerDetails = getOfferDetailsHttpResult.Content.ReadAsAsync<GetOfferByDetailsViewModel>().Result;
            Assert.AreEqual(offersToAdds[0].Title, offerDetails.Title);
            Assert.AreEqual(offersToAdds[0].Description, offerDetails.Description);
            Assert.AreEqual(offersToAdds[0].InitialPrice, offerDetails.InitialPrice);
            //Assert.AreEqual(offersToAdds[0].ExpirationDateTime.ToString(), offerDetails.ExpirationDateTime.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(0, offerDetails.BidsCount);
            Assert.IsTrue(offerDetails.IsExpired);
        }

        [TestMethod]
        public void GetOfferDetails_Of_NonExisting_Offer_Should_Return_404NotFound()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var getOfferDetailsHttpResult = TestingEngine.GetOfferDetailsHttpGet(int.MaxValue);
            Assert.AreEqual(HttpStatusCode.NotFound, getOfferDetailsHttpResult.StatusCode);
        }
    }
}
