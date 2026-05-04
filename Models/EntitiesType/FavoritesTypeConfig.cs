using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class FavoritesTypeConfig : IEntityTypeConfiguration<Favorites>
    {
        public void Configure(EntityTypeBuilder<Favorites> builder)
        {
            //db name 
            builder.ToTable("Favorites");

            //PK 
            builder.HasKey(p => new { p.BoardGame_Id, p.User_Id });

            //prop
            builder.Property(p => p.BoardGame_Id).HasColumnName("boardgame_id").HasColumnType("uniqueidentifier");
            builder.Property(p => p.User_Id).HasColumnName("user_id").HasColumnType("int");
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //relationship
            builder.HasOne(p => p.BoardGame).WithMany(p => p.Favorites).HasForeignKey(p => p.BoardGame_Id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.User).WithMany(p => p.Favorites).HasForeignKey(p => p.BoardGame_Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
