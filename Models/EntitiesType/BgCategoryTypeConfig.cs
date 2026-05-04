using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class BgCategoryTypeConfig:IEntityTypeConfiguration<BoardGameCategory>
    {
        public void Configure(EntityTypeBuilder<BoardGameCategory> builder)
        {
            //tb name
            builder.ToTable("BoardGameCategories");

            //PK 
            builder.HasKey(p => new { p.BoardGame_Id, p.Category_Id });

            //prop 
            builder.Property(p => p.BoardGame_Id).HasColumnName("boardgame_id").HasColumnType("uniqueidentifier");
            builder.Property(p => p.Category_Id).HasColumnName("category_id").HasColumnType("int");
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //Fk
            builder.HasOne(p => p.BoardGame).WithMany(p => p.BoardGameCategories).HasForeignKey(p => p.BoardGame_Id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.Category).WithMany(p => p.BoardGameCategories).HasForeignKey(p => p.Category_Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
