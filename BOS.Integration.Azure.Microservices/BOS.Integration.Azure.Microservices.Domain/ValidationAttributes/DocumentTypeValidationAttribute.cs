using BOS.Integration.Azure.Microservices.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.ValidationAttributes
{
    public class DocumentTypeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strValue = value as string;
            var propretyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            bool result = strValue == GoodsReceivalType.Order;

            return result ? ValidationResult.Success : new ValidationResult($"The field {propretyInfo.Name} must be equal to \"{GoodsReceivalType.Order}\"");
        }
    }
}
