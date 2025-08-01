using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopApp.Migrations
{
    /// <inheritdoc />
    public partial class initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracaoBarbearia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeBarbearia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Site = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Instagram = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Facebook = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WhatsApp = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    HorarioAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HorarioFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    SegundaAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    TercaAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuartaAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuintaAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    SextaAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    SabadoAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    DomingoAberta = table.Column<bool>(type: "INTEGER", nullable: false),
                    SegundaAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SegundaFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TercaAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TercaFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    QuartaAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    QuartaFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    QuintaAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    QuintaFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SextaAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SextaFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SabadoAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SabadoFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    DomingoAbertura = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    DomingoFechamento = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    IntervaloAgendamentoMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    PrazoMinimoAgendamentoDias = table.Column<int>(type: "INTEGER", nullable: false),
                    PrazoMaximoAgendamentoDias = table.Column<int>(type: "INTEGER", nullable: false),
                    PoliticasCancelamento = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoBarbearia", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoBarbearia");
        }
    }
}
