using TestWithEF.Models;
using TestWithEF.ValueObjects;

namespace TestWithEF.Entities
{
    public class Address : ValueObject
    {
        private Address() { }

        public static Result<Address> CreateAddress(string street, string city, string postcode, string country)
        {
            return (street.NotEmpty(), city.NotEmpty(), postcode.NotEmpty(),
                    country.NotEmpty())
                .Apply((s, c, p, r) => new Address
                {
                    Street = s,
                    City = c,
                    Postcode = p,
                    Country = r,
                });
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return Postcode;
            yield return Country;
        }

        public string Street { get; private set; }

        public string City { get; private set; }

        public string Postcode { get; private set; }

        public string Country { get; private set; }

        public override string ToString()
        {
            return "Street: " + Street + " City: " + City + " Postcode: " + Postcode + " Country: " + Country + " ";
        }
    }
}
