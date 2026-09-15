using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaReservas.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTokensRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiracion",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenRestablecimiento",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenExpiracion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenRestablecimiento",
                table: "Usuarios");
        }
    }
}
