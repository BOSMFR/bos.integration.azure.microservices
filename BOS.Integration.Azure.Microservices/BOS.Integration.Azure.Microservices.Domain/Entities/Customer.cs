using System;

namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return string.Format($"Hello {FirstName} {LastName}. {Environment.NewLine}Your email: {Email} {Environment.NewLine}Your phone number: {PhoneNumber}");
        }
    }
}
