namespace BOS.Integration.Azure.Microservices.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public override string ToString()
        {
            return string.Format($"Product {Name} costs {Price} $");
        }
    }
}
