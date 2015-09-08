namespace OnlineShop.Services.CustomValidation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using OnlineShop.Data;

    public class AdCategoriesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Missing categories.");
            }

            List<int> categories = value as List<int>;
            if (categories.Count < 1 || categories.Count > 3)
            {
                return new ValidationResult("Categories should be at least 1 and no more than 3.");
            }

            using (OnlineShopContext data = new OnlineShopContext())
            {
                var categoriesInDb = data.Categories
                    .Select(c => c.Id)
                    .ToList();
                foreach (var category in categories)
                {
                    if (!categoriesInDb.Contains(category))
                    {
                        return new ValidationResult("There is invalid category Id.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}