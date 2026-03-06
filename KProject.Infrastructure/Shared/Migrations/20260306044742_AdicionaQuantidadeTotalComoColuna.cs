using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaQuantidadeTotalComoColuna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quantidade_atual",
                table: "estoques",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantidade_atual",
                table: "estoques");
        }
    }
}
