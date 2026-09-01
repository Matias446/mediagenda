using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mediAgenda.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCedulaToMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Los médicos ya no tienen acceso a la app: se eliminan las cuentas
            // de Usuario que quedaron con el rol Medico (valor 2, retirado del enum).
            migrationBuilder.Sql("DELETE FROM \"Usuarios\" WHERE \"Rol\" = 2;");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Medicos_MedicoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_MedicoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Medicos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_Cedula",
                table: "Medicos",
                column: "Cedula",
                unique: true,
                filter: "\"Cedula\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Medicos_Cedula",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Medicos");

            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_MedicoId",
                table: "Usuarios",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Medicos_MedicoId",
                table: "Usuarios",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id");
        }
    }
}
