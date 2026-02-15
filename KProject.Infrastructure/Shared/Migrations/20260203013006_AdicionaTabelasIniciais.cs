using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTabelasIniciais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cliente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "produto",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    codigo_anvisa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_produto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "venda",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    criada_por = table.Column<int>(type: "integer", nullable: false),
                    criada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_venda", x => x.id);
                    table.ForeignKey(
                        name: "fk_venda_asp_net_users_criada_por",
                        column: x => x.criada_por,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_venda_cliente_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lote",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    produto_id = table.Column<int>(type: "integer", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    validade = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lote", x => x.id);
                    table.ForeignKey(
                        name: "fk_lote_produto_produto_id",
                        column: x => x.produto_id,
                        principalTable: "produto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estoque",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lote_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estoque", x => x.id);
                    table.ForeignKey(
                        name: "fk_estoque_lote_lote_id",
                        column: x => x.lote_id,
                        principalTable: "lote",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_consignado",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venda_id = table.Column<int>(type: "integer", nullable: false),
                    lote_id = table.Column<int>(type: "integer", nullable: false),
                    quantidade_consignada = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_consignado", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_consignado_lote_lote_id",
                        column: x => x.lote_id,
                        principalTable: "lote",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_consignado_venda_venda_id",
                        column: x => x.venda_id,
                        principalTable: "venda",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historico_estoque",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estoque_id = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delta_quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_historico_estoque", x => x.id);
                    table.ForeignKey(
                        name: "fk_historico_estoque_estoque_estoque_id",
                        column: x => x.estoque_id,
                        principalTable: "estoque",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historico_quantidade",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_consignado_id = table.Column<int>(type: "integer", nullable: false),
                    devolvido = table.Column<long>(type: "bigint", nullable: false),
                    vendido = table.Column<long>(type: "bigint", nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    alterado_por = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_historico_quantidade", x => x.id);
                    table.ForeignKey(
                        name: "fk_historico_quantidade_asp_net_users_alterado_por",
                        column: x => x.alterado_por,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_historico_quantidade_item_consignado_item_consignado_id",
                        column: x => x.item_consignado_id,
                        principalTable: "item_consignado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cliente_nome",
                table: "cliente",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_estoque_lote_id",
                table: "estoque",
                column: "lote_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_historico_estoque_estoque_id_criado_em",
                table: "historico_estoque",
                columns: new[] { "estoque_id", "criado_em" });

            migrationBuilder.CreateIndex(
                name: "ix_historico_quantidade_alterado_por",
                table: "historico_quantidade",
                column: "alterado_por");

            migrationBuilder.CreateIndex(
                name: "ix_historico_quantidade_item_consignado_id_alterado_em",
                table: "historico_quantidade",
                columns: new[] { "item_consignado_id", "alterado_em" });

            migrationBuilder.CreateIndex(
                name: "ix_item_consignado_lote_id",
                table: "item_consignado",
                column: "lote_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_consignado_venda_id_lote_id",
                table: "item_consignado",
                columns: new[] { "venda_id", "lote_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lote_produto_id",
                table: "lote",
                column: "produto_id");

            migrationBuilder.CreateIndex(
                name: "ix_produto_codigo",
                table: "produto",
                column: "codigo");

            migrationBuilder.CreateIndex(
                name: "ix_produto_criado_em",
                table: "produto",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "ix_venda_cliente_id",
                table: "venda",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_venda_criada_em",
                table: "venda",
                column: "criada_em");

            migrationBuilder.CreateIndex(
                name: "ix_venda_criada_por",
                table: "venda",
                column: "criada_por");

            migrationBuilder.CreateIndex(
                name: "ix_venda_status",
                table: "venda",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historico_estoque");

            migrationBuilder.DropTable(
                name: "historico_quantidade");

            migrationBuilder.DropTable(
                name: "estoque");

            migrationBuilder.DropTable(
                name: "item_consignado");

            migrationBuilder.DropTable(
                name: "lote");

            migrationBuilder.DropTable(
                name: "venda");

            migrationBuilder.DropTable(
                name: "produto");

            migrationBuilder.DropTable(
                name: "cliente");
        }
    }
}
