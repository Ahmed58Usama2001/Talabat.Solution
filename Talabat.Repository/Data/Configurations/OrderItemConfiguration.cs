using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities.Order_Aggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Talabat.Repository.Data.Configurations;

internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(O => O.Product, Product => Product.WithOwner()); //[1:1] Total Participation

        builder.Property(O => O.Price)
            .HasColumnType("decimal(18,2)");
    }
}
