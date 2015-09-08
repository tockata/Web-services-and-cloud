namespace BookShop.Services.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AddOrChangeCategoryModelBinder
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
    }
}