using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class CartsTypeConfig : IEntityTypeConfiguration<Carts>
    {
        public void Configure(EntityTypeBuilder<Carts> builder)
        {
            //tb name 
            builder.ToTable("Carts");

            //id
            builder.HasKey(x => x.Id);

            //Config
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("BIGINT").UseIdentityColumn(1, 1);
            builder.Property(p => p.UserId).HasColumnName("user_id").HasColumnType("INT").IsRequired();
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //Relation ship 1-1 , 1 user only have one cart 
            builder.HasOne(p => p.User).WithOne().HasForeignKey<Carts>(p=>p.UserId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
