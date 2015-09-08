namespace BugTracker.RestServices.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Http;

    using BugTracker.Data;
    using BugTracker.Data.Contracts;
    using BugTracker.Data.Models;
    using BugTracker.RestServices.Models.Bugs;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/bugs")]
    public class BugsController : BaseApiController
    {
        public BugsController()
            : this(new BugTrackerData(new BugTrackerDbContext()))
        {
        }

        public BugsController(IBugTrackerData data)
            : base(data)
        {
        }

        [HttpGet]
        public IHttpActionResult GetAllBugs()
        {
            var bugs = this.Data.Bugs.All()
                .OrderByDescending(b => b.DateCreated)
                .Select(GetAllBugsViewModel.Create);

            return this.Ok(bugs);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBugDetailsById(int id)
        {
            
            var bugInfo = this.Data.Bugs.All()
                .Where(b => b.Id == id)
                .Select(GetBugDetailsByIdViewModel.Create)
                .FirstOrDefault();

            if (bugInfo == null)
            {
                return this.NotFound();
            }

            return this.Ok(bugInfo);
        }

        [HttpPost]
        public IHttpActionResult PostNewBug(PostNewBugBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model is null.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newBug = new Bug
            {
                Title = model.Title,
                Description = model.Description,
                Status = Status.Open,
                DateCreated = DateTime.Now
            };

            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != null)
            {

                var userInDb = this.Data.Users.All()
                    .FirstOrDefault(u => u.Id == loggedUserId);
                if (userInDb == null)
                {
                    return this.BadRequest("Invalid token");
                }

                newBug.AuthorId = loggedUserId;
                this.Data.Bugs.Add(newBug);
                this.Data.SaveChanges();

                return this.CreatedAtRoute(
                    "DefaultApi", 
                    new { Id = newBug.Id }, 
                    new
                    {
                        Id = newBug.Id,
                        Author = userInDb.UserName,
                        Message = "User bug submitted."
                    });
            }

            this.Data.Bugs.Add(newBug);
            this.Data.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { Id = newBug.Id },
                new
                {
                    Id = newBug.Id,
                    Message = "Anonymous bug submitted."
                });
        }

        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult EditExistingBug(int id, EditBugBindingModel model)
        {
            var bugInDb = this.Data.Bugs.All().FirstOrDefault(b => b.Id == id);
            if (bugInDb == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Missing bug properties to change.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Invalid bug properties.");
            }

            var modelStatus = (Status)Enum.Parse(typeof(Status), model.Status);

            bugInDb.Title = model.Title ?? bugInDb.Title;
            bugInDb.Description = model.Description ?? bugInDb.Description;
            bugInDb.Status = modelStatus != bugInDb.Status ? modelStatus : bugInDb.Status;

            this.Data.Bugs.Update(bugInDb);
            this.Data.SaveChanges();
            return this.Ok(new { Message = "Bug #" + id + " patched." });
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteExistingBug(int id)
        {
            var bugInDb = this.Data.Bugs.All().FirstOrDefault(b => b.Id == id);
            if (bugInDb == null)
            {
                return this.NotFound();
            }

            this.Data.Bugs.Delete(bugInDb);
            this.Data.SaveChanges();
            return this.Ok(new { Message = "Bug #" + id + " deleted." });
        }

        [HttpGet]
        [Route("filter")]
        public IHttpActionResult GetBugsByFilter([FromUri]GetBugsByFilterBindingModel model)
        {
            string[] splittedStatuses = model.Statuses.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var bugs = this.Data.Bugs.All()
                .OrderByDescending(b => b.DateCreated)
                .Include(b => b.Author);

            if (model.Keyword != null)
            {
                bugs = bugs.Where(b => b.Title.Contains(model.Keyword));
            }

            if (model.Author != null)
            {
                bugs = bugs.Where(b => b.Author.UserName == model.Author);
            }

            if (splittedStatuses.Any())
            {
                bugs = bugs.Where(b => splittedStatuses.Contains(b.Status.ToString()));
            }

            var bugsView = bugs.ToList().Select(b => new GetAllBugsViewModel
            {
                Title = b.Title,
                Status = b.Status.ToString(),
                Author = b.Author != null ? b.Author.UserName : null,
                DateCreated = b.DateCreated
            });
            return this.Ok(bugsView);
        }
    }
}
