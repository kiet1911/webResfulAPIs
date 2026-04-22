using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.EntitiesType
{
    public class ProfilesTypeConfig: IEntityTypeConfiguration<Profiles>
    {
        public void Configure( EntityTypeBuilder<Profiles> builder)
        {
            //table name 
            builder.ToTable("Profiles");

            //PK
            builder.HasKey(p=>p.Id);

            //prop
            builder.Property(p => p.Id).HasColumnName("Id").HasColumnType("int").UseIdentityColumn(1, 1);
            builder.Property(p => p.Full_Name).HasColumnName("full_name").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.Display_Name).HasColumnName("display_name").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.Image_Url).HasColumnName("img_url").HasColumnType("nvarchar(256)");
            builder.Property(p => p.Birth).HasColumnName("birth").HasColumnType("dateTime").IsRequired().HasDefaultValue(new DateTime(1999, 1, 1)); ;
            builder.Property(p => p.Gender).HasColumnName("gender").HasColumnType("varchar(20)").HasConversion<string>().HasDefaultValue(EnumStores.UserGender.Male);
            builder.Property(p => p.Address).HasColumnName("address").HasColumnType("nvarchar(256)");
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("dateTime").HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("dateTime").HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.User_Id).HasColumnName("user_id").HasColumnType("int").IsRequired();

            //FK
            builder.HasOne(p => p.User).WithOne().HasForeignKey<Profiles>(p => p.User_Id).OnDelete(DeleteBehavior.Restrict);

            //index
            builder.HasIndex(p => p.User_Id).IsUnique().HasDatabaseName("IX_Profiles_User_Id");

            //constraint
            builder.ToTable("Profiles", t =>
            {
                t.HasCheckConstraint("CK_Profiles_Gender", "gender IN ('Male','Female','Others')");
            });
        }
    }
}
