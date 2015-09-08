namespace BugTracker.RestServices.Models.Comments
{
    using System;
    using System.Linq.Expressions;

    using BugTracker.Data.Models;

    public class BugDetailsCommentViewModel
    {
        public static Expression<Func<Comment, BugDetailsCommentViewModel>> Create
        {
            get
            {
                return c => new BugDetailsCommentViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    Author = c.Author != null ? c.Author.UserName : null,
                    DateCreated = c.DateCreated
                };
            }
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }
    }
}