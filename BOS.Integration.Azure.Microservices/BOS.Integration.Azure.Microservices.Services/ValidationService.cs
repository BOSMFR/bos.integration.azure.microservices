using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class ValidationService : IValidationService
    {
        public List<ValidationResult> Validate<T>(T model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            Validator.TryValidateObject(model, context, results, true);

            return results;
        }
    }
}
