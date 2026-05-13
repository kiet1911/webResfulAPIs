using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.EntitiesType
{
    public class CreatorstypeConfig : IEntityTypeConfiguration<Creators>
    {
        public void Configure(EntityTypeBuilder<Creators> builder)
        {
            //tb name
            builder.ToTable("Creators");

            //PK 
            builder.HasKey(p => p.Id);

            //prop
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.Name).HasColumnName("name").HasColumnType("nvarchar(256)").IsRequired().HasDefaultValue("empty");
            builder.Property(p => p.Bio).HasColumnName("bio").HasColumnType("nvarchar(256)").HasDefaultValue(string.Empty);
            builder.Property(p => p.Type).HasColumnName("type").HasColumnType("nvarchar(100)").IsRequired().HasConversion<string>().HasDefaultValue(EnumStores.CreatorTypes.Author);
            builder.Property(p => p.Status).HasColumnName("status").HasColumnType("nvarchar(100)").IsRequired().HasConversion<string>().HasDefaultValue(EnumStores.CreatorStatus.Active);
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //constraint
            builder.ToTable("Creators", t =>
            {
                t.HasCheckConstraint("CK_Creators_Type", "type IN ('Author','Artist')");
                t.HasCheckConstraint("CK_Creators_Status", "status IN ('Active', 'Inactive', 'Banned')");
            });


        }
    }
}
