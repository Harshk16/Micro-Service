using Microsoft.EntityFrameworkCore;
using Payment.Service.Features;

namespace Payment.Service.Infrastructure.Persistence.Configurations;


public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentItem> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentItemConfiguration());
    }
}