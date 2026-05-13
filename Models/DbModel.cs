using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Models.EntitiesType;

namespace webResfulAPIs.Models
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Profiles> Profiles { get; set; }
        public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }
        public virtual DbSet<BoardGames> BoardGames { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { 
        
        }

        protected override void OnModelCreating( ModelBuilder builder )
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
