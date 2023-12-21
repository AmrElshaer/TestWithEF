using Microsoft.EntityFrameworkCore;
using TestWithEF.Configurations;
using TestWithEF.Entities;

namespace TestWithEF
{
    public class TestDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<Order> Orders { get; set; }

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
