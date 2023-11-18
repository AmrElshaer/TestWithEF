using CSharpFunctionalExtensions;
using TestWithEF.Entities;

namespace TestWithEF.ValueObjects
{
    public class ContactDetails : ValueObject
    {
        private ContactDetails() { }

        public static Result<ContactDetails> CreateContactDetails(string phone, Address address)
        {
            if (string.IsNullOrEmpty(phone))
                return Result.Failure<ContactDetails>("phone number is empty");

            return new ContactDetails()
            {
                Phone = phone,
                Address = address
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Phone;
            yield return Address;
        }

        public Address Address { get; private set; }

        public string Phone { get; private set; }
    }
}
