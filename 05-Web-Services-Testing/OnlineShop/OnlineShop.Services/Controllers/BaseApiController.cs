namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;

    using OnlineShop.Data.Contracts;
    using OnlineShop.Services.Infrastructure;

    public class BaseApiController : ApiController
    {
        public BaseApiController(IOnlineShopData data, IUserIdProvider userIdProvider)
        {
            this.Data = data;
            this.UserIdProvider = userIdProvider;
        }

        public IOnlineShopData Data { get; set; }

        public IUserIdProvider UserIdProvider { get; set; }
    }
}
