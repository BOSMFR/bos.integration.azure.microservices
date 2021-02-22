namespace BOS.Integration.Azure.Microservices.Domain
{
    public class ActionExecutionResult
    {
        public bool Succeeded { get; set; } = false;

        public object Entity { get; set; }

        public string Error { get; set; }
    }
}
