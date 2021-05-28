using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IValidationService
    {
        List<ValidationResult> Validate<T>(T model);
    }
}
