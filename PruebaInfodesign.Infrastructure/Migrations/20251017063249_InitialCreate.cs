using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PruebaInfodesign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lineas",
                columns: table => new
                {
                    LineaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoLinea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineas", x => x.LineaId);
                });

            migrationBuilder.CreateTable(
                name: "TiposCliente",
                columns: table => new
                {
                    TipoClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCliente", x => x.TipoClienteId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosConsumo",
                columns: table => new
                {
                    RegistroId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineaId = table.Column<int>(type: "int", nullable: false),
                    TipoClienteId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    ConsumoWh = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    PorcentajePerdida = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosConsumo", x => x.RegistroId);
                    table.CheckConstraint("CK_ConsumoPositivo", "[ConsumoWh] >= 0");
                    table.CheckConstraint("CK_CostoPositivo", "[CostoUnitario] >= 0");
                    table.CheckConstraint("CK_PerdidaValida", "[PorcentajePerdida] >= 0 AND [PorcentajePerdida] <= 100");
                    table.ForeignKey(
                        name: "FK_RegistroConsumo_Linea",
                        column: x => x.LineaId,
                        principalTable: "Lineas",
                        principalColumn: "LineaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroConsumo_TipoCliente",
                        column: x => x.TipoClienteId,
                        principalTable: "TiposCliente",
                        principalColumn: "TipoClienteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TiposCliente",
                columns: new[] { "TipoClienteId", "Descripcion", "NombreTipo" },
                values: new object[,]
                {
                    { 1, "Clientes residenciales o domésticos", "Residencial" },
                    { 2, "Clientes comerciales y empresariales", "Comercial" },
                    { 3, "Clientes industriales de alto consumo", "Industrial" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lineas_CodigoLinea",
                table: "Lineas",
                column: "CodigoLinea",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistroConsumo_Fecha",
                table: "RegistrosConsumo",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroConsumo_FechaCompleto",
                table: "RegistrosConsumo",
                columns: new[] { "Fecha", "LineaId", "TipoClienteId" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroConsumo_LineaFecha",
                table: "RegistrosConsumo",
                columns: new[] { "LineaId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroConsumo_TipoClienteFecha",
                table: "RegistrosConsumo",
                columns: new[] { "TipoClienteId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_TiposCliente_Nombre",
                table: "TiposCliente",
                column: "NombreTipo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosConsumo");

            migrationBuilder.DropTable(
                name: "Lineas");

            migrationBuilder.DropTable(
                name: "TiposCliente");
        }
    }
}
