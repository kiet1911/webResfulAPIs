using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class RefreshTokensTypeConfig : IEntityTypeConfiguration<RefreshTokens>
    {
        public void Configure(EntityTypeBuilder<RefreshTokens> builder)
        {
            //table name
            builder.ToTable("RefreshTokens");

            //PK
            builder.HasKey(p => p.Id);

            //prop
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("int").UseIdentityColumn(1,1);
            builder.Property(p => p.User_id).HasColumnName("user_id").HasColumnType("int");
            builder.Property(p => p.Token).HasColumnName("token").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.Expired_date).HasColumnName("expired_date").HasColumnType("datetime").IsRequired();
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired();
            builder.Property(p => p.Created_by_ip).HasColumnName("created_by_ip").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.Is_revoked).HasColumnName("is_revoked").HasColumnType("bit").HasDefaultValue(false);

            //FK
            builder.HasOne(p=>p.User).WithMany(u=>u.RefreshTokens).HasForeignKey(p=>p.User_id).OnDelete(DeleteBehavior.Restrict);


            //constraint

            //index 
            builder.HasIndex(p => p.Token).IsUnique().HasDatabaseName("IX_RefreshTokens_Token");
        }
    }
}
