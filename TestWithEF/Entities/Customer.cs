using CSharpFunctionalExtensions;

namespace TestWithEF.Entities
{
    public class Customer:Entity<Guid>
    {
        private Customer(string name)
        {
            this.Name = name;
        }
        public Customer(string name, CustomerAddress address)
            : this(name)
        {
            this.Address = address;
        }
        public string Name { get;private set; }   
        public CustomerAddress Address { get;private set; }
    }
    public class CustomerAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public Money Money { get; set; }
    }

   public record Money(int value);
}
