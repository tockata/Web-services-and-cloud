namespace News.Services.Controllers
{
    using System.Web.Http;

    using News.Data;
    using News.Data.Contracts;

    public class BaseApiController : ApiController
    {
        public BaseApiController()
            : this(new NewsData(new NewsContext()))
        {
        }

        public BaseApiController(INewsData data)
        {
            this.Data = data;
        }

        public INewsData Data { get; set; }
    }
}
