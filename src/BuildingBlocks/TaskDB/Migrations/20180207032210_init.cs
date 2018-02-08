using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dy.TaskDB.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_user",
                columns: table => new
                {
                    uid = table.Column<Guid>(nullable: false),
                    create_time = table.Column<DateTime>(nullable: false),
                    email = table.Column<string>(maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user", x => x.uid);
                });

            migrationBuilder.CreateTable(
                name: "tbl_task",
                columns: table => new
                {
                    tid = table.Column<Guid>(nullable: false),
                    cron_expression = table.Column<string>(maxLength: 100, nullable: false),
                    cron_remark = table.Column<string>(maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    is_dce = table.Column<bool>(nullable: false),
                    last_runtime = table.Column<DateTime>(nullable: true),
                    runcount = table.Column<int>(nullable: false),
                    task_desc = table.Column<string>(maxLength: 300, nullable: false),
                    task_key = table.Column<string>(maxLength: 20, nullable: false),
                    task_name = table.Column<string>(maxLength: 50, nullable: false),
                    task_url = table.Column<string>(maxLength: 100, nullable: false),
                    uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_task", x => x.tid);
                    table.ForeignKey(
                        name: "user_fk_uid",
                        column: x => x.uid,
                        principalTable: "tbl_user",
                        principalColumn: "uid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_task_uid",
                table: "tbl_task",
                column: "uid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_task");

            migrationBuilder.DropTable(
                name: "tbl_user");
        }
    }
}
