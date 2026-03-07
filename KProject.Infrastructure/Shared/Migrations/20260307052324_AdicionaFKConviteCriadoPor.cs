using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaFKConviteCriadoPor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_convites_criado_por_id",
                table: "convites",
                column: "criado_por_id");

            migrationBuilder.AddForeignKey(
                name: "fk_convites_asp_net_users_criado_por_id",
                table: "convites",
                column: "criado_por_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_convites_asp_net_users_criado_por_id",
                table: "convites");

            migrationBuilder.DropIndex(
                name: "ix_convites_criado_por_id",
                table: "convites");
        }
    }
}
