using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumina.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updatetag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryTag_Tag_TagId",
                table: "JournalEntryTag");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemDefined",
                table: "Tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Tag",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TagId",
                table: "JournalEntryTag",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserTagId",
                table: "JournalEntryTag",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTag_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryTag_UserTagId",
                table: "JournalEntryTag",
                column: "UserTagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTag_UserId",
                table: "UserTag",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryTag_Tag_TagId",
                table: "JournalEntryTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryTag_UserTag_UserTagId",
                table: "JournalEntryTag",
                column: "UserTagId",
                principalTable: "UserTag",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryTag_Tag_TagId",
                table: "JournalEntryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryTag_UserTag_UserTagId",
                table: "JournalEntryTag");

            migrationBuilder.DropTable(
                name: "UserTag");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryTag_UserTagId",
                table: "JournalEntryTag");

            migrationBuilder.DropColumn(
                name: "IsSystemDefined",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UserTagId",
                table: "JournalEntryTag");

            migrationBuilder.AlterColumn<int>(
                name: "TagId",
                table: "JournalEntryTag",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryTag_Tag_TagId",
                table: "JournalEntryTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
