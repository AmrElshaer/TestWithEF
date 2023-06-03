using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestWithEF.Entities;

namespace TestWithEF.Configurations;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(product => product.Id).ValueGeneratedNever();
        builder.Property(product => product.Name).HasMaxLength(100);
        builder.HasDiscriminator<ProductType>("ProductType")
            .HasValue<StandardProduct>(ProductType.Standard)
            .HasValue<FeaturedProduct>(ProductType.Featured);
    }
}
