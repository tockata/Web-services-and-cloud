namespace BugTracker.Data.Models
{
    using System;

    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        public DateTime DateCreated { get; set; }

        public int BugId { get; set; }

        public virtual Bug Bug { get; set; }
    }
}
