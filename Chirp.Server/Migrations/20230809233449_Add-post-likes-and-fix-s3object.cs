using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Server.Migrations
{
    /// <inheritdoc />
    public partial class Addpostlikesandfixs3object : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "attachment_id",
                table: "s3objects",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "post_like",
                columns: table => new
                {
                    profile_id = table.Column<long>(type: "bigint", nullable: false),
                    post_id = table.Column<long>(type: "bigint", nullable: false),
                    liked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_like", x => new { x.profile_id, x.post_id });
                    table.ForeignKey(
                        name: "fk_post_like_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_like_profiles_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profiles",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_s3objects_attachment_id",
                table: "s3objects",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_like_post_id",
                table: "post_like",
                column: "post_id");

            migrationBuilder.AddForeignKey(
                name: "fk_s3objects_attachments_attachment_id",
                table: "s3objects",
                column: "attachment_id",
                principalTable: "attachments",
                principalColumn: "attachment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_s3objects_attachments_attachment_id",
                table: "s3objects");

            migrationBuilder.DropTable(
                name: "post_like");

            migrationBuilder.DropIndex(
                name: "ix_s3objects_attachment_id",
                table: "s3objects");

            migrationBuilder.DropColumn(
                name: "attachment_id",
                table: "s3objects");
        }
    }
}
