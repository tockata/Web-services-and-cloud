namespace BugTracker.RestServices.Controllers
{
    using System.Web.Http;
    using BugTracker.Data;
    using BugTracker.Data.Contracts;

    public class BaseApiController : ApiController
    {
        public BaseApiController()
            : this(new BugTrackerData(new BugTrackerDbContext()))
        {
        }

        public BaseApiController(IBugTrackerData data)
        {
            this.Data = data;
        }

        public IBugTrackerData Data { get; set; }
    }
}
