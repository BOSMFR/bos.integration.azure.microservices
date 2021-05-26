using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.ValidationAttributes
{
    public class IsInteger64ValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strValue = value as string;
            var propretyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            bool result = string.IsNullOrEmpty(strValue) ? false : long.TryParse(strValue, out long _);

            return result ? ValidationResult.Success : new ValidationResult($"The field {propretyInfo.Name} must be an Int64 value");
        }
    }
}
