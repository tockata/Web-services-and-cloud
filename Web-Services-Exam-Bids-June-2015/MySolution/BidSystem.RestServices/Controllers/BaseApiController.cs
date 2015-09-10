namespace BidSystem.RestServices.Controllers
{
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Contracts;
    using BidSystem.RestServices.Infrastructure;

    public class BaseApiController : ApiController
    {
        public BaseApiController()
            : this(new BidSystemData(new BidSystemDbContext()), new AspNetUserIdProvider())
        {
        }

        public BaseApiController(IBidSystemData data, IUserIdProvider userIdProvider)
        {
            this.Data = data;
            this.UserIdProvider = userIdProvider;
        }

        public IBidSystemData Data { get; set; }

        public IUserIdProvider UserIdProvider { get; set; }
    }
}
