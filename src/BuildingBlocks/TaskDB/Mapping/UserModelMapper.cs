using Dy.TaskUtility.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dy.TaskDB.Mapping
{
    public class UserModelMapper : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ToTable("tbl_user");
            builder.HasKey(e => e.UID);
            builder.Property<Guid>(p => p.UID).HasColumnName("uid");
            builder.Property<string>(p => p.Email).HasColumnName("email").IsRequired().HasMaxLength(50);
            builder.Property<string>(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
            builder.Property<DateTime>(p => p.CreateTime).HasColumnName("create_time").IsRequired();
            builder.Property<bool>(e => e.IsActive).HasColumnName("is_active");
        }
    }
}
