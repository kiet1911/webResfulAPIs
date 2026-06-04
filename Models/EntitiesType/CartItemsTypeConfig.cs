using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webResfulAPIs.Models.EntitiesType
{
    public class CartItemsTypeConfig : IEntityTypeConfiguration<CartItems>
    {
        public void Configure( EntityTypeBuilder<CartItems> builder)
        {
            //
            builder.ToTable("CartItems");

            //pk
            builder.HasKey(x => new { x.CartId, x.BoardgameId });

            //Property
            builder.Property(p => p.CartId).HasColumnName("cart_id").HasColumnType("BIGINT").IsRequired();
            builder.Property(p => p.BoardgameId).HasColumnName("boardgame_id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(p => p.Quantity).HasColumnName("quantity").HasColumnType("INT").IsRequired().HasDefaultValue(1);
            builder.Property(p => p.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");

            //relationship
            builder.HasOne(p => p.Cart).WithMany(p => p.CartItems).HasForeignKey(p => p.CartId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.BoardGames).WithMany(p => p.CartItems).HasForeignKey(p => p.BoardgameId).OnDelete(DeleteBehavior.Restrict);

            //

            //constraint
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CartItems_Quantity", "quantity > 0 and quantity <= 99");
                t.HasCheckConstraint("CK_CartItems_UnitPrice", "unit_price > 0");
            });

        }
    }
}
