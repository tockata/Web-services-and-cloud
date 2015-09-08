namespace BugTracker.RestServices.Models.Bugs
{
    public class EditBugBindingModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }
}