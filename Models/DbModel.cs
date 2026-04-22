using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Models.EntitiesType;

namespace webResfulAPIs.Models
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Users> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { 
        
        }

        protected override void OnModelCreating( ModelBuilder builder )
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
