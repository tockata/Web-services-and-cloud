namespace BookShop.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using BookShop.Services.Validators;
    using BookShopSystem.Models;

    public class EditBookBindingModel
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Copies { get; set; }

        [Required]
        [ValidEnumValue]
        public EditionType Edition { get; set; }

        [Required]
        [ValidEnumValue]
        public AgeRestriction AgeRestriction { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string AuthorId { get; set; }
    }
}