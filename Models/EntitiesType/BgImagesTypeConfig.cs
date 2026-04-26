using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class BgImagesTypeConfig : IEntityTypeConfiguration<BoardGameImages>
    {
        public void Configure(EntityTypeBuilder<BoardGameImages> builder)
        {
            //  db name 
            builder.ToTable("BoardGameImages");

            //  PK
            builder.HasKey(p => p.Id);

            //prop
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.Img_Url).HasColumnName("img_url").HasColumnType("nvarchar(256)").IsRequired().HasDefaultValue("");
            builder.Property(p => p.Alt).HasColumnName("alt").HasColumnType("nvarchar(256)").IsRequired().HasDefaultValue("");
            builder.Property(p => p.Is_Thumbnail).HasColumnName("is_thumbnail").HasColumnType("bit").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Bg_id).HasColumnName("bg_id").HasColumnType("uniqueidentifier").IsRequired();

            //constraint
            builder.HasOne(p=> p.BoardGame).WithMany(p=>p.BoardGameImages).HasForeignKey(p=>p.Bg_id).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
