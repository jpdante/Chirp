using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    account_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.account_id);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    profile_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    account_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    handle = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    biography = table.Column<string>(type: "text", nullable: true),
                    profile_picture = table.Column<string>(type: "text", nullable: true),
                    background_picture = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profiles", x => x.profile_id);
                    table.ForeignKey(
                        name: "fk_profiles_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_email",
                table: "accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profiles_account_id",
                table: "profiles",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_handle",
                table: "profiles",
                column: "handle",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profiles");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
