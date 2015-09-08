namespace BugTracker.RestServices.Models.Bugs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BugTracker.Data.Models;
    using BugTracker.RestServices.Models.Comments;

    public class GetBugDetailsByIdViewModel
    {
        public static Expression<Func<Bug, GetBugDetailsByIdViewModel>> Create
        {
            get
            {
                return b => new GetBugDetailsByIdViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Status = b.Status.ToString(),
                    Author = b.Author == null ? b.Author.UserName : null,
                    DateCreated = b.DateCreated,
                    Comments = b.Comments.
                        OrderByDescending(c => c.DateCreated)
                        .Select(c => new BugDetailsCommentViewModel
                        {
                            Id = c.Id,
                            Text = c.Text,
                            Author = c.Author == null ? c.Author.UserName : null,
                            DateCreated = c.DateCreated
                        })
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }

        public IEnumerable<BugDetailsCommentViewModel> Comments { get; set; }
    }
}