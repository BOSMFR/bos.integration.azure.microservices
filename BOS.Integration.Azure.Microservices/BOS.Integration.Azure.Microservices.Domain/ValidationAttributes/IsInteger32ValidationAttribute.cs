using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.ValidationAttributes
{
    public class IsInteger32ValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strValue = value as string;
            var propretyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            bool result = string.IsNullOrEmpty(strValue) ? false : int.TryParse(strValue, out int _);

            return result ? ValidationResult.Success : new ValidationResult($"The field {propretyInfo.Name} must be an Int32 value");
        }
    }
}
