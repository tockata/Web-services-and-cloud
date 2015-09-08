namespace News.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class NewsBindingModel
    {
        [Required]
        [MinLength(5)]
        public string Title { get; set; }

        [Required]
        [MinLength(10)]
        public string Content { get; set; }

        [Column(TypeName = "DateTime2")]
        [Required]
        public DateTime PublishDate { get; set; }
    }
}