using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using System.Numerics;

namespace TestWithEF.Entities
{
    public class Address:ValueObject
    {
        private Address() { }
        public static Result<Address> CreateAddress(string street, string city, string postcode, string country)
        {
            if (string.IsNullOrEmpty(street)) return Result.Failure<Address>("street  is empty");
            if (string.IsNullOrEmpty(city)) return Result.Failure<Address>("city  is empty");
            if (string.IsNullOrEmpty(postcode)) return Result.Failure<Address>("postcode  is empty");
            if (string.IsNullOrEmpty(country)) return Result.Failure<Address>("country is empty");
            return new Address()
            {
                Street = street,
                City = city,
                Postcode = postcode,
                Country = country
            };

        }

        protected override IEnumerable<object> GetEqualityComponents()
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
