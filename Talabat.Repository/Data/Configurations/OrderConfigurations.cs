using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Configurations;

internal class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(O => O.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner()); //[1:1] Total Participation

        builder.Property(o => o.Status)
            .HasConversion
            (
            OStatus=>OStatus.ToString(),
            OStatus=> (OrderStatus)Enum.Parse(typeof(OrderStatus),OStatus)
            );

        builder.Property(O => O.SubTotal)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(O => O.DeliveryMethod)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
    }
}
