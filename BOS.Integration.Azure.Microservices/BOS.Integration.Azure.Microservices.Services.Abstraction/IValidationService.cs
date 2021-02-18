namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IValidationService
    {
        bool Validate<T>(T model);
    }
}
