using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaNomeProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "produto",
                newName: "referencia");

            migrationBuilder.RenameIndex(
                name: "ix_produto_codigo",
                table: "produto",
                newName: "ix_produto_referencia");

            migrationBuilder.AddColumn<string>(
                name: "nome",
                table: "produto",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_produto_nome",
                table: "produto",
                column: "nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_produto_nome",
                table: "produto");

            migrationBuilder.DropColumn(
                name: "nome",
                table: "produto");

            migrationBuilder.RenameColumn(
                name: "referencia",
                table: "produto",
                newName: "codigo");

            migrationBuilder.RenameIndex(
                name: "ix_produto_referencia",
                table: "produto",
                newName: "ix_produto_codigo");
        }
    }
}
