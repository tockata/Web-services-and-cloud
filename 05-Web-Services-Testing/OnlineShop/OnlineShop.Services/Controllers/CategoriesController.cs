namespace OnlineShop.Services.Controllers
{
    using OnlineShop.Data.Contracts;
    using OnlineShop.Services.Infrastructure;

    public class CategoriesController : BaseApiController
    {
        public CategoriesController(IOnlineShopData data, IUserIdProvider userIdProvider)
            : base(data, userIdProvider)
        {
        }
    }
}
