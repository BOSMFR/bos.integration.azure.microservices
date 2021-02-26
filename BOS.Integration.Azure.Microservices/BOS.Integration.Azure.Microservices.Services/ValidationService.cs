using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class ValidationService : IValidationService
    {
        public bool Validate<T>(T model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            bool result = Validator.TryValidateObject(model, context, results, true);

            return result;
        }
    }
}
