using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.ValidationAttributes
{
    public class BarcodeValidationAttribute : ValidationAttribute
    {
        private const string BarcodeStartString = "57";

        public override bool IsValid(object value)
        {
            var barcode = value as string;

            if (!barcode.StartsWith(BarcodeStartString))
            {
                this.ErrorMessage = $"The string should start with '{BarcodeStartString}'";
                return false;
            }
            return true;
        }
    }
}
