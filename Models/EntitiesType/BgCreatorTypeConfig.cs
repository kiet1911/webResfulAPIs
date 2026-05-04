using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class BgCreatorTypeConfig:IEntityTypeConfiguration<BoardGameCreators>
    {
        public void Configure( EntityTypeBuilder<BoardGameCreators> builder)
        {
            //bg name 
            builder.ToTable("BoardGameCreators");

            //PK 
            builder.HasKey(p => new { p.Creator_Id, p.BoardGame_Id });

            //props
            builder.Property(p => p.BoardGame_Id).HasColumnName("boardgame_id").IsRequired().HasColumnType("uniqueidentifier");
            builder.Property(p=>p.Creator_Id).HasColumnName("creator_id").IsRequired().HasColumnType("uniqueidentifier");
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            // relationship 
            builder.HasOne(p => p.Creator).WithMany(p => p.BoardGameCreators).HasForeignKey(p => p.Creator_Id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.BoardGame).WithMany(p => p.BoardGameCreators).HasForeignKey(p => p.BoardGame_Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
