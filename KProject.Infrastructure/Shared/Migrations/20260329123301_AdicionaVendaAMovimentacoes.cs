using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaVendaAMovimentacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "venda_id",
                table: "historico_estoque",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_historico_estoque_venda_id",
                table: "historico_estoque",
                column: "venda_id");

            migrationBuilder.AddForeignKey(
                name: "fk_historico_estoque_vendas_venda_id",
                table: "historico_estoque",
                column: "venda_id",
                principalTable: "vendas",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_historico_estoque_vendas_venda_id",
                table: "historico_estoque");

            migrationBuilder.DropIndex(
                name: "ix_historico_estoque_venda_id",
                table: "historico_estoque");

            migrationBuilder.DropColumn(
                name: "venda_id",
                table: "historico_estoque");
        }
    }
}
