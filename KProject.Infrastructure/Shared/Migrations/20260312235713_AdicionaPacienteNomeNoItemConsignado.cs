using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaPacienteNomeNoItemConsignado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_item_consignado_venda_id_lote_id",
                table: "item_consignado");

            migrationBuilder.AddColumn<string>(
                name: "paciente_nome",
                table: "item_consignado",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_item_consignado_venda_id_lote_id_paciente_nome",
                table: "item_consignado",
                columns: new[] { "venda_id", "lote_id", "paciente_nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_item_consignado_venda_id_lote_id_paciente_nome",
                table: "item_consignado");

            migrationBuilder.DropColumn(
                name: "paciente_nome",
                table: "item_consignado");

            migrationBuilder.CreateIndex(
                name: "ix_item_consignado_venda_id_lote_id",
                table: "item_consignado",
                columns: new[] { "venda_id", "lote_id" },
                unique: true);
        }
    }
}
