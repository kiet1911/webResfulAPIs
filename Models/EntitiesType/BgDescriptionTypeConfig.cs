using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class BgDescriptionTypeConfig : IEntityTypeConfiguration<BoardGameDescription>
    {
        public void Configure(EntityTypeBuilder<BoardGameDescription> builder)
        {
            // db name 
            builder.ToTable("BoardGameDescription");

            // PK 
            builder.HasKey(p => p.Id);

            //prop
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("int").UseIdentityColumn(1, 1);
            builder.Property(p => p.Short_Description).HasColumnName("short_description").HasColumnType("nvarchar(max)").IsRequired().HasDefaultValue(string.Empty);
            builder.Property(p => p.Full_Description).HasColumnName("full_description").HasColumnType("nvarchar(max)").IsRequired().HasDefaultValue(string.Empty);
            builder.Property(p => p.BoardGame_Id).HasColumnName("bg_id").HasColumnType("uniqueidentifier");
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //FK 
            builder.HasOne(p => p.BoardGames).WithOne().HasForeignKey<BoardGameDescription>(p => p.BoardGame_Id).OnDelete(DeleteBehavior.Restrict);

            //constraint

        }
    }
}
