using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mediAgenda.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_PacienteId",
                table: "Turnos");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_PacienteId_MedicoId_FechaHora",
                table: "Turnos",
                columns: new[] { "PacienteId", "MedicoId", "FechaHora" },
                unique: true,
                filter: "\"Estado\" IN (0, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_PacienteId_MedicoId_FechaHora",
                table: "Turnos");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_PacienteId",
                table: "Turnos",
                column: "PacienteId");
        }
    }
}
