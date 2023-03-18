using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TestWithEF.Configurations;
using TestWithEF.Entities;

namespace TestWithEF
{
    public class TestContext : DbContext
    {
       public DbSet<Author> Authors { get; set; }
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
           
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorConfiguration).Assembly);
        }

    }
}
