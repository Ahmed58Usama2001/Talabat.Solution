using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Configurations
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(e=>e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.PictureUrl)
              .IsRequired();

            builder.Property(e => e.Description)
              .IsRequired();

            builder.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(e => e.Brand)
                .WithMany()
                .HasForeignKey(e => e.BrandId);

            builder.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);
        }
    }
}
