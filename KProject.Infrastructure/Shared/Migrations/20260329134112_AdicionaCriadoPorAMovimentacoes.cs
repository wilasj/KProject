using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCriadoPorAMovimentacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "criado_por",
                table: "historico_estoque",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "criado_por",
                table: "historico_estoque");
        }
    }
}
