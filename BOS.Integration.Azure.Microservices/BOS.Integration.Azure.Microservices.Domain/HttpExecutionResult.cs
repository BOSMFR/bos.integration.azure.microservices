namespace BOS.Integration.Azure.Microservices.Domain
{
    public class HttpExecutionResult
    {
        public bool Succeeded { get; set; } = false;

        public string Content{ get; set; }

        public string Error { get; set; }
    }
}
