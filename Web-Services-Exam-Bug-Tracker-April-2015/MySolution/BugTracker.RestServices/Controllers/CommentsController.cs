namespace BugTracker.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using BugTracker.Data.Models;
    using BugTracker.RestServices.Models.Bugs;
    using BugTracker.RestServices.Models.Comments;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api")]
    public class CommentsController : BaseApiController
    {
        [HttpGet]
        [Route("comments")]
        public IHttpActionResult GetAllComments()
        {
            var comments = this.Data.Comments.All()
                .OrderByDescending(c => c.DateCreated)
                .Select(GetAllCommentsViewModel.Create);

            return this.Ok(comments);
        }

        [HttpGet]
        [Route("bugs/{id}/comments")]
        public IHttpActionResult GetCommentsForGivenBug(int id)
        {
            var bug = this.Data.Bugs.All().FirstOrDefault(b => b.Id == id);
            if (bug == null)
            {
                return this.NotFound();
            }

            var comments = this.Data.Comments.All()
                .Where(c => c.BugId == id)
                .OrderByDescending(c => c.DateCreated)
                .Select(BugDetailsCommentViewModel.Create);

            return this.Ok(comments);
        }

        [HttpPost]
        [Route("bugs/{id}/comments")]
        public IHttpActionResult PostNewCommentToGivenBug(int id, PostNewCommentBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model is null.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var bugInDb = this.Data.Bugs.All().FirstOrDefault(b => b.Id == id);
            if (bugInDb == null)
            {
                return this.NotFound();
            }

            var newComment = new Comment
            {
                Text = model.Text,
                DateCreated = DateTime.Now,
                BugId = bugInDb.Id
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

                newComment.AuthorId = loggedUserId;
                this.Data.Comments.Add(newComment);
                this.Data.SaveChanges();

                bugInDb.Comments.Add(newComment);
                userInDb.Comments.Add(newComment);
                this.Data.SaveChanges();

                return this.Ok(new
                {
                    Id = newComment.Id,
                    Author = userInDb.UserName,
                    Message = "User comment added for bug #" + newComment.Id
                });
            }

            this.Data.Comments.Add(newComment);
            this.Data.SaveChanges();

            bugInDb.Comments.Add(newComment);
            this.Data.SaveChanges();

            return this.Ok(new
            {
                Id = newComment.Id,
                Message = "Added anonymous comment for bug #" + newComment.Id
            });
        }
    }
}
