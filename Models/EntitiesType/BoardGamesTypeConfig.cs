using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.EntitiesType
{
    public class BoardGamesTypeConfig : IEntityTypeConfiguration<BoardGames>
    {
        public void Configure(EntityTypeBuilder<BoardGames> builder)
        {
            //tb name
            builder.ToTable("BoardGames");
            //pk
            builder.HasKey(x => x.Id);
            //prop
            builder.Property(p => p.Id).HasColumnName("id").HasColumnType("uniqueidentifier").HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(p => p.Name).HasColumnName("name").HasColumnType("nvarchar(256)").IsRequired().HasDefaultValue(string.Empty);
            builder.Property(p => p.Base_Price).HasColumnName("base_price").HasColumnType("decimal(18, 2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Stock_Quantity).HasColumnName("stock_quantity").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Sold_Quantity).HasColumnName("sold_quantity").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Created_at).HasColumnName("created_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Updated_at).HasColumnName("updated_at").HasColumnType("datetime").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.Status).HasColumnName("status").HasColumnType("nvarchar(100)").IsRequired().HasConversion<string>().HasDefaultValue(EnumStores.BoardGameStatus.Active);
            builder.Property(p => p.Weight).HasColumnName("weight").HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Size_X).HasColumnName("size_x").HasColumnType("decimal(8,2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Size_Y).HasColumnName("size_y").HasColumnType("decimal(8,2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Size_Z).HasColumnName("size_z").HasColumnType("decimal(8,2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Min_Player).HasColumnName("min_player").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Max_Player).HasColumnName("max_player").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Min_Time).HasColumnName("min_time").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Max_Time).HasColumnName("max_time").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Prefer_Player).HasColumnName("prefer_player").HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(p => p.Complexity).HasColumnName("complexity").HasColumnType("decimal(4,2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Rating).HasColumnName("rating").HasColumnType("decimal(4,2)").IsRequired().HasDefaultValue(0.00);
            builder.Property(p => p.Age_Requirement).HasColumnName("age_requirement").HasColumnType("int").IsRequired().HasDefaultValue(0);

            //constraint 
            builder.ToTable( t =>
            {
                t.HasCheckConstraint("CK_BoardGames_Price", "base_price >= 0");
                t.HasCheckConstraint("CK_BoardGames_Stock", "stock_quantity >= 0");
                t.HasCheckConstraint("CK_BoardGames_Sold", "sold_quantity >= 0");
                t.HasCheckConstraint("CK_BoardGames_Weight", "weight >= 0");
                t.HasCheckConstraint("CK_BoardGames_SizeX", "size_x >= 0");
                t.HasCheckConstraint("CK_BoardGames_SizeY", "size_y >= 0");
                t.HasCheckConstraint("CK_BoardGames_SizeZ", "size_z >= 0");
                t.HasCheckConstraint("CK_BoardGames_ValidPlayer", "max_player >= min_player and min_player >=0");
                t.HasCheckConstraint("CK_BoardGames_ValidTime", "max_time >= min_time and min_time >=0");
                t.HasCheckConstraint("CK_BoardGames_Prefer", "prefer_player >= 0");
                t.HasCheckConstraint("CK_BoardGames_Rating", "rating >= 0 and rating <=10");
                t.HasCheckConstraint("CK_BoardGames_Age", "age_requirement >= 0");
            });
        }
    }
}
