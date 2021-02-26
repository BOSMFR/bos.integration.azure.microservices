using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.ValidationAttributes
{
    public class TypeIdValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string typeId = value as string;

            if (typeId != "B" && typeId != "F")
            {
                this.ErrorMessage = $"The string should be equal to 'B' or 'F'";
                return false;
            }

            return true;
        }
    }
}
