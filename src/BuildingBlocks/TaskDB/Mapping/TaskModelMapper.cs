using Dy.TaskUtility.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dy.TaskDB.Mapping
{
    public class TaskModelMapper : IEntityTypeConfiguration<TaskModel>
    {
        public void Configure(EntityTypeBuilder<TaskModel> builder)
        {
            builder.ToTable("tbl_task");
            builder.HasKey(e => e.TaskID);
            builder.Property<Guid>(p => p.TaskID).HasColumnName("tid");
            builder.Property<string>(p => p.TaskName).HasColumnName("task_name").IsRequired().HasMaxLength(50);
            builder.Property<string>(p => p.TaskDesc).HasColumnName("task_desc").IsRequired().HasMaxLength(300);
            builder.Property<string>(p => p.TaskKey).HasColumnName("task_key").IsRequired().HasMaxLength(20);
            builder.Property<string>(p => p.TaskUrl).HasColumnName("task_url").IsRequired().HasMaxLength(100);
            builder.Property<string>(p => p.CronExpressionString).HasColumnName("cron_expression").IsRequired().HasMaxLength(100);
            builder.Property<string>(p => p.CronRemark).HasColumnName("cron_remark").IsRequired().HasMaxLength(100);
            builder.Property<bool>(p => p.IsActive).HasColumnName("is_active");
            builder.Property<bool>(p => p.IsDce).HasColumnName("is_dce");

            builder.Property<int>(p => p.RunCount).HasColumnName("runcount");
            builder.Property<DateTime?>(p => p.LastRunTime).HasColumnName("last_runtime");

            builder.Property<Guid>(p => p.UID).HasColumnName("uid");
            builder.HasOne(p => p.CreateUser).WithMany(p => p.TaskList).HasForeignKey(p => p.UID).HasConstraintName("user_fk_uid");
        }
    }
}
