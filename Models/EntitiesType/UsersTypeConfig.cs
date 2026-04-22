using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.EntitiesType
{
    public class UsersTypeConfig : IEntityTypeConfiguration<Users>
    {
        
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            //name table
            builder.ToTable("Users");

            //PK
            builder.HasKey(p => p.Id);

            //name for each prop
            builder.Property(p=>p.Id).HasColumnName("id").HasColumnType("int").UseIdentityColumn(1,1);
            builder.Property(p => p.Public_id).HasColumnName("public_id").HasColumnType("nvarchar(100)");
            builder.Property(p => p.Email).HasColumnName("email").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.Password).HasColumnName("password_hash").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.Phone).HasColumnName("phone").HasColumnType("varchar(20)");
            builder.Property(p => p.Email_verify).HasColumnName("email_verify").HasColumnType("DateTime");
            builder.Property(p => p.Status).HasColumnName("status").HasColumnType("nvarchar(100)").HasConversion<string>().HasDefaultValue(EnumStores.UserStatus.Active);
            builder.Property(p => p.Role).HasColumnName("role").HasColumnType("nvarchar(100)");
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("DateTime").HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("DateTime").HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Deleted_at).HasColumnName("deleted_at").HasColumnType("DateTime");

            //constraint
            builder.ToTable("Users", t =>
            {
                t.HasCheckConstraint("CK_Users_Status", "status IN ('Active','Inactive','Pending','Banned')");
                t.HasCheckConstraint("CK_Users_Role", "role IN ('Admin','Users','Staff')");
            });
            //index 
            builder.HasIndex(p => p.Public_id).IsUnique().HasDatabaseName("IX_Users_Public_id");
            builder.HasIndex(p => p.Email).IsUnique().HasDatabaseName("IX_Users_Email");
            builder.HasIndex(p => p.Phone).IsUnique().HasDatabaseName("IX_Users_Phone");

        }
    }
}
