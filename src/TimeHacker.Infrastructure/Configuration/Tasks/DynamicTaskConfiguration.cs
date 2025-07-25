﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Infrastructure.Configuration.Tasks
{
    public class DynamicTaskConfiguration : IEntityTypeConfiguration<DynamicTask>
    {
        public void Configure(EntityTypeBuilder<DynamicTask> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedTimestamp);

            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).HasMaxLength(516);
            builder.Property(x => x.Priority).IsRequired();
            builder.Property(x => x.MinTimeToFinish).IsRequired();
            builder.Property(x => x.MaxTimeToFinish).IsRequired();
            builder.Property(x => x.CreatedTimestamp).IsRequired();
        }
    }
}
