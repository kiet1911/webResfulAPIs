using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class OrderItemsTypeConfig:IEntityTypeConfiguration<OrderItems>
    {
        public void Configure (EntityTypeBuilder<OrderItems> builder)
        {
            //
            builder.ToTable("OrderItems");

            //
            builder.HasKey(p => p.Id);

            //
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.OrderId).HasColumnName("order_id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(p => p.BoardgameId).HasColumnName("boardgame_id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(p => p.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Quantity).HasColumnName("quantity").HasColumnType("INT").IsRequired();
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //relationship
            builder.HasOne(p => p.Order).WithMany(p => p.OrderItems).HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.BoardGame).WithMany(p => p.OrderItems).HasForeignKey(p => p.BoardgameId).OnDelete(DeleteBehavior.Restrict);

            //constraint
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_OrderItems_UnitPrice", "unit_price >=0");
                t.HasCheckConstraint("CK_OrderItems_Quantity", "quantity > 0");
            });
        }
    }
}
