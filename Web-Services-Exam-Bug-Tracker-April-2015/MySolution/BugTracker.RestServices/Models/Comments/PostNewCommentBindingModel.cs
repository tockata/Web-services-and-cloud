namespace BugTracker.RestServices.Models.Comments
{
    using System.ComponentModel.DataAnnotations;

    public class PostNewCommentBindingModel
    {
        [Required]
        public string Text { get; set; }
    }
}
