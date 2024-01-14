using Microsoft.EntityFrameworkCore;
using TestWithEF.Configurations;
using TestWithEF.Entities;
using TestWithEF.Features.Orders;

namespace TestWithEF
{
    public partial class TestDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.State)
                .HasConversion(
                    s => s.GetType().Name,
                    s => GetOrderState(s)
                );

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderProducts)
                .WithOne()
                .HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(op => op.ProductId);

            modelBuilder.Entity<OrderProduct>()
                .HasKey(o => new
                {
                    o.ProductId,
                    o.OrderId
                });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorConfiguration).Assembly);
        }

        private static OrderState GetOrderState(string state)

        {
            return state switch
            {
                nameof(DraftState) => new DraftState(),
                nameof(ConfirmedState) => new ConfirmedState(),
                nameof(UnderProcessingState) => new UnderProcessingState(),
                nameof(CancelledState) => new CancelledState(),
                _ => throw new InvalidOperationException($"Unknown state: {state}")
            };
        }
    }
}
