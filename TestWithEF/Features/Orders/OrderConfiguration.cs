using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestWithEF.Entities;

namespace TestWithEF.Features.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasOne<Author>()
            .WithMany()
            .HasForeignKey(o => o.AuthorId);
    }
}
