﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using System.Drawing;

namespace TimeHacker.Infrastructure.Configuration.ScheduleSnapshots
{
    public class ScheduleCategoryConfiguration : IEntityTypeConfiguration<ScheduledCategory>
    {
        public void Configure(EntityTypeBuilder<ScheduledCategory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ScheduleSnapshot).WithMany(x => x.ScheduledCategories)
                   .HasForeignKey(x => new { x.UserId, x.Date }).HasPrincipalKey(x => new { x.UserId, x.Date })
                   .OnDelete(DeleteBehavior.ClientCascade);

            builder.Property(x => x.Color).IsRequired().HasConversion(
                v => v.ToArgb(),
                v => Color.FromArgb(v)
            );
        }
    }
}
