using Microsoft.EntityFrameworkCore;
using TestWithEF.Features.Orders;

// ReSharper disable once CheckNamespace
namespace TestWithEF;

public partial class TestDbContext
{
    public DbSet<Order> Orders { get; set; }
}
