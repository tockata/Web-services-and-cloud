namespace BugTracker.RestServices.Models.Bugs
{
    using System;
    using System.Linq.Expressions;

    using BugTracker.Data.Models;

    public class GetAllCommentsViewModel
    {
        public static Expression<Func<Comment, GetAllCommentsViewModel>> Create
        {
            get
            {
                return c => new GetAllCommentsViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    Author = c.Author == null ? c.Author.UserName : null,
                    DateCreated = c.DateCreated,
                    BugId = c.BugId,
                    BugTitle = c.Bug.Title
                };
            }
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }

        public int BugId { get; set; }

        public string BugTitle { get; set; }
    }
}