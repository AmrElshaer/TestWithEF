using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TestWithEf.StateDP
{
    public class StateDPContext: DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public StateDPContext(DbContextOptions<StateDPContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
           .Property(o => o.State)
           .HasConversion(
               s => s.GetType().Name,
               s =>string.IsNullOrEmpty(s)?null:(OrderState)Activator.CreateInstance(Type.GetType($"TestWithEf.StateDP.{s}"))
           );
           
        }
    }
}
