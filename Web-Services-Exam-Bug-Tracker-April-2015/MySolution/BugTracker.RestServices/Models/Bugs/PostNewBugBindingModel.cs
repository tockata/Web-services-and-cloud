namespace BugTracker.RestServices.Models.Bugs
{
    using System.ComponentModel.DataAnnotations;

    public class PostNewBugBindingModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
