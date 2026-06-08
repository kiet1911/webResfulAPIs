using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.EntitiesType
{
    public class OrdersTypeConfig:IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            //
            builder.ToTable("Orders");

            //
            builder.HasKey(x => x.Id);

            //
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.CartId).HasColumnName("cart_id").HasColumnType("BIGINT").IsRequired();
            builder.Property(p => p.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Status).HasColumnName("status").HasColumnType("nvarchar(100)").IsRequired().HasConversion<string>().HasDefaultValue(EnumStores.OrderStatus.Pending);
            builder.Property(p => p.IsSuccessDelivery).HasColumnName("isSuccess_delivery").HasColumnType("BIT").IsRequired().HasConversion<bool>().HasDefaultValue(0);
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Paid_at).HasColumnName("paid_at").HasColumnType("datetime");
            builder.Property(p => p.Deleted_at).HasColumnName("deleted_at").HasColumnType("datetime");

            //relationship 
            builder.HasOne(p => p.Cart).WithMany(p => p.Orders).HasForeignKey(p => p.CartId).OnDelete(DeleteBehavior.Restrict);

            //constraint
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Orders_ToTalPrice", "total_price >= 0");
                t.HasCheckConstraint("CK_Orders_Status", "status in ('Pending','Confirmed','Cancelled','Refunded','Shipping', 'Delivered') ");

            });
        }
    }
}
