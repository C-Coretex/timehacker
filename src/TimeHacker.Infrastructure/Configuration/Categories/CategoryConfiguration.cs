﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Drawing;
using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Infrastructure.Configuration.Categories
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.UserId).HasMaxLength(450);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Color).IsRequired().HasConversion(
                v => v.ToArgb(),
                v => Color.FromArgb(v)
            );
        }
    }
}
