using CSharpFunctionalExtensions;
using TestWithEF.ValueObjects;

namespace TestWithEF.Entities
{
    public class Author : Entity<Guid>
    {
        // To prevent user from create instance using new Author()
        private Author() { }

        public static Author CreateAuthor(AuthorName authorName, ContactDetails contactDetails)
        {
            var author = new Author()
            {
                Id = Guid.NewGuid(),
                Name = authorName,
                ContactDetails = contactDetails
            };

            return author;
        }

        public string Name { get; private set; }

        public ContactDetails ContactDetails { get; private set; }

        public Author UpdateName(AuthorName authorName)
        {
            this.Name = authorName;

            return this;
        }

        public Author UpdateAuthor(AuthorName authorName, ContactDetails contactDetails)
        {
            this.Name = authorName;
            this.ContactDetails = contactDetails;

            return this;
        }
    }
}
