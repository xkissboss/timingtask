﻿// <auto-generated />
using Dy.TaskDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Dy.TaskDB.Migrations
{
    [DbContext(typeof(TaskDbContext))]
    [Migration("20180207032210_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Dy.TaskUtility.Models.TaskModel", b =>
                {
                    b.Property<Guid>("TaskID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("tid");

                    b.Property<string>("CronExpressionString")
                        .IsRequired()
                        .HasColumnName("cron_expression")
                        .HasMaxLength(100);

                    b.Property<string>("CronRemark")
                        .IsRequired()
                        .HasColumnName("cron_remark")
                        .HasMaxLength(100);

                    b.Property<bool>("IsActive")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDce")
                        .HasColumnName("is_dce");

                    b.Property<DateTime?>("LastRunTime")
                        .HasColumnName("last_runtime");

                    b.Property<int>("RunCount")
                        .HasColumnName("runcount");

                    b.Property<string>("TaskDesc")
                        .IsRequired()
                        .HasColumnName("task_desc")
                        .HasMaxLength(300);

                    b.Property<string>("TaskKey")
                        .IsRequired()
                        .HasColumnName("task_key")
                        .HasMaxLength(20);

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasColumnName("task_name")
                        .HasMaxLength(50);

                    b.Property<string>("TaskUrl")
                        .IsRequired()
                        .HasColumnName("task_url")
                        .HasMaxLength(100);

                    b.Property<Guid>("UID")
                        .HasColumnName("uid");

                    b.HasKey("TaskID");

                    b.HasIndex("UID");

                    b.ToTable("tbl_task");
                });

            modelBuilder.Entity("Dy.TaskUtility.Models.UserModel", b =>
                {
                    b.Property<Guid>("UID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("uid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnName("create_time");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email")
                        .HasMaxLength(50);

                    b.Property<bool>("IsActive")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(50);

                    b.HasKey("UID");

                    b.ToTable("tbl_user");
                });

            modelBuilder.Entity("Dy.TaskUtility.Models.TaskModel", b =>
                {
                    b.HasOne("Dy.TaskUtility.Models.UserModel", "CreateUser")
                        .WithMany("TaskList")
                        .HasForeignKey("UID")
                        .HasConstraintName("user_fk_uid")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
