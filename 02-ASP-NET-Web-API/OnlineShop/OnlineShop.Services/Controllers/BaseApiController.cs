namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;

    using OnlineShop.Data;

    public class BaseApiController : ApiController
    {
        public BaseApiController()
            : this(new OnlineShopContext())
        {
        }

        public BaseApiController(OnlineShopContext data)
        {
            this.Data = data;
        }

        public OnlineShopContext Data { get; set; }
    }
}
