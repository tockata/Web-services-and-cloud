namespace BugTracker.RestServices.Models.Bugs
{
    using System;
    using System.Linq.Expressions;

    using BugTracker.Data.Models;

    public class GetAllBugsViewModel
    {
        public static Expression<Func<Bug, GetAllBugsViewModel>> Create
        {
            get
            {
                return b => new GetAllBugsViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Status = b.Status.ToString(),
                    Author = b.Author == null ? b.Author.UserName : null,
                    DateCreated = b.DateCreated
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }
    }
}