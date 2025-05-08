using DeliveryApplicationBackend.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }  // Add more entities as needed
    public DbSet<PickupLocation> PickupLocations { get; set; }
}
