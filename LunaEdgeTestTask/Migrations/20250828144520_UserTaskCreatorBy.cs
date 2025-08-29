using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaEdgeTestTask.Migrations
{
    /// <inheritdoc />
    public partial class UserTaskCreatorBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tasks",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Tasks",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                newName: "IX_Tasks_userId");

            migrationBuilder.AlterColumn<Guid>(
                name: "userId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_userId",
                table: "Tasks",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_userId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Tasks",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Tasks",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_userId",
                table: "Tasks",
                newName: "IX_Tasks_UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
