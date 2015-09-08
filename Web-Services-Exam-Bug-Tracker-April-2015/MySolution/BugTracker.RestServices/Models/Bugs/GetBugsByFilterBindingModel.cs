namespace BugTracker.RestServices.Models.Bugs
{
    public class GetBugsByFilterBindingModel
    {
        public string Keyword { get; set; }

        public string Statuses { get; set; }

        public string Author { get; set; }
    }
}