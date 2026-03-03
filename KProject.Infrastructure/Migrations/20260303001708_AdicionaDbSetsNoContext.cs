using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaDbSetsNoContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_estoque_lote_lote_id",
                table: "estoque");

            migrationBuilder.DropForeignKey(
                name: "fk_historico_estoque_estoque_estoque_id",
                table: "historico_estoque");

            migrationBuilder.DropForeignKey(
                name: "fk_item_consignado_lote_lote_id",
                table: "item_consignado");

            migrationBuilder.DropForeignKey(
                name: "fk_item_consignado_venda_venda_id",
                table: "item_consignado");

            migrationBuilder.DropForeignKey(
                name: "fk_lote_produto_produto_id",
                table: "lote");

            migrationBuilder.DropForeignKey(
                name: "fk_venda_asp_net_users_criada_por",
                table: "venda");

            migrationBuilder.DropForeignKey(
                name: "fk_venda_cliente_cliente_id",
                table: "venda");

            migrationBuilder.DropPrimaryKey(
                name: "pk_venda",
                table: "venda");

            migrationBuilder.DropPrimaryKey(
                name: "pk_produto",
                table: "produto");

            migrationBuilder.DropPrimaryKey(
                name: "pk_lote",
                table: "lote");

            migrationBuilder.DropPrimaryKey(
                name: "pk_estoque",
                table: "estoque");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cliente",
                table: "cliente");

            migrationBuilder.RenameTable(
                name: "venda",
                newName: "vendas");

            migrationBuilder.RenameTable(
                name: "produto",
                newName: "produtos");

            migrationBuilder.RenameTable(
                name: "lote",
                newName: "lotes");

            migrationBuilder.RenameTable(
                name: "estoque",
                newName: "estoques");

            migrationBuilder.RenameTable(
                name: "cliente",
                newName: "clientes");

            migrationBuilder.RenameIndex(
                name: "ix_venda_status",
                table: "vendas",
                newName: "ix_vendas_status");

            migrationBuilder.RenameIndex(
                name: "ix_venda_criada_por",
                table: "vendas",
                newName: "ix_vendas_criada_por");

            migrationBuilder.RenameIndex(
                name: "ix_venda_criada_em",
                table: "vendas",
                newName: "ix_vendas_criada_em");

            migrationBuilder.RenameIndex(
                name: "ix_venda_cliente_id",
                table: "vendas",
                newName: "ix_vendas_cliente_id");

            migrationBuilder.RenameIndex(
                name: "ix_produto_referencia",
                table: "produtos",
                newName: "ix_produtos_referencia");

            migrationBuilder.RenameIndex(
                name: "ix_produto_nome",
                table: "produtos",
                newName: "ix_produtos_nome");

            migrationBuilder.RenameIndex(
                name: "ix_produto_criado_em",
                table: "produtos",
                newName: "ix_produtos_criado_em");

            migrationBuilder.RenameIndex(
                name: "ix_lote_produto_id",
                table: "lotes",
                newName: "ix_lotes_produto_id");

            migrationBuilder.RenameIndex(
                name: "ix_estoque_lote_id",
                table: "estoques",
                newName: "ix_estoques_lote_id");

            migrationBuilder.RenameIndex(
                name: "ix_cliente_nome",
                table: "clientes",
                newName: "ix_clientes_nome");

            migrationBuilder.AddPrimaryKey(
                name: "pk_vendas",
                table: "vendas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_produtos",
                table: "produtos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_lotes",
                table: "lotes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_estoques",
                table: "estoques",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_clientes",
                table: "clientes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_estoques_lotes_lote_id",
                table: "estoques",
                column: "lote_id",
                principalTable: "lotes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_historico_estoque_estoques_estoque_id",
                table: "historico_estoque",
                column: "estoque_id",
                principalTable: "estoques",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_consignado_lotes_lote_id",
                table: "item_consignado",
                column: "lote_id",
                principalTable: "lotes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_consignado_vendas_venda_id",
                table: "item_consignado",
                column: "venda_id",
                principalTable: "vendas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_lotes_produtos_produto_id",
                table: "lotes",
                column: "produto_id",
                principalTable: "produtos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_vendas_asp_net_users_criada_por",
                table: "vendas",
                column: "criada_por",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vendas_clientes_cliente_id",
                table: "vendas",
                column: "cliente_id",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_estoques_lotes_lote_id",
                table: "estoques");

            migrationBuilder.DropForeignKey(
                name: "fk_historico_estoque_estoques_estoque_id",
                table: "historico_estoque");

            migrationBuilder.DropForeignKey(
                name: "fk_item_consignado_lotes_lote_id",
                table: "item_consignado");

            migrationBuilder.DropForeignKey(
                name: "fk_item_consignado_vendas_venda_id",
                table: "item_consignado");

            migrationBuilder.DropForeignKey(
                name: "fk_lotes_produtos_produto_id",
                table: "lotes");

            migrationBuilder.DropForeignKey(
                name: "fk_vendas_asp_net_users_criada_por",
                table: "vendas");

            migrationBuilder.DropForeignKey(
                name: "fk_vendas_clientes_cliente_id",
                table: "vendas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_vendas",
                table: "vendas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_produtos",
                table: "produtos");

            migrationBuilder.DropPrimaryKey(
                name: "pk_lotes",
                table: "lotes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_estoques",
                table: "estoques");

            migrationBuilder.DropPrimaryKey(
                name: "pk_clientes",
                table: "clientes");

            migrationBuilder.RenameTable(
                name: "vendas",
                newName: "venda");

            migrationBuilder.RenameTable(
                name: "produtos",
                newName: "produto");

            migrationBuilder.RenameTable(
                name: "lotes",
                newName: "lote");

            migrationBuilder.RenameTable(
                name: "estoques",
                newName: "estoque");

            migrationBuilder.RenameTable(
                name: "clientes",
                newName: "cliente");

            migrationBuilder.RenameIndex(
                name: "ix_vendas_status",
                table: "venda",
                newName: "ix_venda_status");

            migrationBuilder.RenameIndex(
                name: "ix_vendas_criada_por",
                table: "venda",
                newName: "ix_venda_criada_por");

            migrationBuilder.RenameIndex(
                name: "ix_vendas_criada_em",
                table: "venda",
                newName: "ix_venda_criada_em");

            migrationBuilder.RenameIndex(
                name: "ix_vendas_cliente_id",
                table: "venda",
                newName: "ix_venda_cliente_id");

            migrationBuilder.RenameIndex(
                name: "ix_produtos_referencia",
                table: "produto",
                newName: "ix_produto_referencia");

            migrationBuilder.RenameIndex(
                name: "ix_produtos_nome",
                table: "produto",
                newName: "ix_produto_nome");

            migrationBuilder.RenameIndex(
                name: "ix_produtos_criado_em",
                table: "produto",
                newName: "ix_produto_criado_em");

            migrationBuilder.RenameIndex(
                name: "ix_lotes_produto_id",
                table: "lote",
                newName: "ix_lote_produto_id");

            migrationBuilder.RenameIndex(
                name: "ix_estoques_lote_id",
                table: "estoque",
                newName: "ix_estoque_lote_id");

            migrationBuilder.RenameIndex(
                name: "ix_clientes_nome",
                table: "cliente",
                newName: "ix_cliente_nome");

            migrationBuilder.AddPrimaryKey(
                name: "pk_venda",
                table: "venda",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_produto",
                table: "produto",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_lote",
                table: "lote",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_estoque",
                table: "estoque",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cliente",
                table: "cliente",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_estoque_lote_lote_id",
                table: "estoque",
                column: "lote_id",
                principalTable: "lote",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_historico_estoque_estoque_estoque_id",
                table: "historico_estoque",
                column: "estoque_id",
                principalTable: "estoque",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_consignado_lote_lote_id",
                table: "item_consignado",
                column: "lote_id",
                principalTable: "lote",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_consignado_venda_venda_id",
                table: "item_consignado",
                column: "venda_id",
                principalTable: "venda",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_lote_produto_produto_id",
                table: "lote",
                column: "produto_id",
                principalTable: "produto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_venda_asp_net_users_criada_por",
                table: "venda",
                column: "criada_por",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_venda_cliente_cliente_id",
                table: "venda",
                column: "cliente_id",
                principalTable: "cliente",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
