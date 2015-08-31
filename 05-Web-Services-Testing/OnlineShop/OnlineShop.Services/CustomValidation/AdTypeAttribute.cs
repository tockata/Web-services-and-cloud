namespace OnlineShop.Services.CustomValidation
{
    using System.ComponentModel.DataAnnotations;

    using OnlineShop.Data;

    public class AdTypeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int id = (int)value;

            if (id == 0)
            {
                return new ValidationResult("Missing ad type.");
            }

            using (OnlineShopContext data = new OnlineShopContext())
            {
                var adType = data.AdTypes.Find(id);
                if (adType == null)
                {
                    return new ValidationResult("No such ad type in database.");
                }
            }

            return ValidationResult.Success;
        }
    }
}