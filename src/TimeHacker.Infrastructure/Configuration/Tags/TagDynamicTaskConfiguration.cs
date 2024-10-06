﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeHacker.Domain.Contracts.Entities.Tags;

namespace TimeHacker.Infrastructure.Configuration.Tags
{
    public class TagDynamicTaskConfiguration : IEntityTypeConfiguration<TagDynamicTask>
    {
        public void Configure(EntityTypeBuilder<TagDynamicTask> builder)
        {
            builder.HasKey(x => new { x.TagId, x.TaskId });

            builder.HasOne(x => x.Tag).WithMany(x => x.TagDynamicTasks)
                .HasForeignKey(x => x.TagId).HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Task).WithMany(x => x.TagDynamicTasks)
                .HasForeignKey(x => x.TaskId).HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
