using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.EntitiesType
{
    public class CategoriesTypeConfig : IEntityTypeConfiguration<Categories>
    {
        public void Configure(EntityTypeBuilder<Categories> builder)
        {
            //db name 
            builder.ToTable("Categories");

            //PK
            builder.HasKey(p => p.Id);

            //prop
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("int").UseIdentityColumn(1, 1);
            builder.Property(p => p.Name).HasColumnName("name").HasColumnType("nvarchar(256)").IsRequired().HasDefaultValue(string.Empty);
            builder.Property(p => p.Description).HasColumnName("description").HasColumnType("nvarchar(max)").IsRequired().HasDefaultValue(string.Empty);
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Status).HasColumnName("status").HasColumnType("varchar(100)").HasConversion<string>().IsRequired().HasDefaultValue(EnumStores.CategoryStatus.Active);

            //constraint
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Category_Status", "status IN ('Active', 'Inactive', 'Banned')");
            });

        }
    }
}
