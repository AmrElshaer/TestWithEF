using TestWithEF.Entities;
using TestWithEF.Models;

namespace TestWithEF.ValueObjects
{
    public class ContactDetails : ValueObject
    {
        private ContactDetails() { }

        public static Result<ContactDetails> CreateContactDetails(string phone, Address address)
        {
            return phone.NotEmpty().Map(p => new ContactDetails()
            {
                Phone = phone,
                Address = address
            });
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Phone;
            yield return Address;
        }

        public Address Address { get; private set; }

        public string Phone { get; private set; }
    }
}
