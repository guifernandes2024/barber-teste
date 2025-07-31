using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProfissionalFieldsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profissionais_Email",
                table: "Profissionais");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Profissionais");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Profissionais");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Profissionais");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Profissionais",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Profissionais",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Profissionais",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_Email",
                table: "Profissionais",
                column: "Email",
                unique: true);
        }
    }
}
