using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mediAgenda.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTokensValidosDesdeToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokensValidosDesde",
                table: "Usuarios",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokensValidosDesde",
                table: "Usuarios");
        }
    }
}
