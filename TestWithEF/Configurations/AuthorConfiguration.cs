using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using TestWithEF.Entities;

namespace TestWithEF.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.Property(author => author.Id).ValueGeneratedNever();
            builder.Property(author => author.Name).HasMaxLength(100).IsRequired();
            builder.HasIndex(author => author.Name).IsUnique();
            builder.Property(author => author.Name).IsRequired();;
            builder.OwnsOne(
               author => author.ContactDetails, ownedNavigationBuilder =>
               {
                   ownedNavigationBuilder.ToJson();
                   ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
               });
        }
    }
}
