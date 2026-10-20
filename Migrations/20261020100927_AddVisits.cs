using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitnessEf.Migrations
{
    /// <inheritdoc />
    public partial class AddVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "clients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "active",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "registration_date",
                table: "clients",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "visits",
                columns: table => new
                {
                    visit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    visit_date = table.Column<DateOnly>(type: "date", nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visits", x => x.visit_id);
                    table.ForeignKey(
                        name: "FK_visits_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_clients_status",
                table: "clients",
                sql: "\"status\" IN ('active', 'inactive')");

            migrationBuilder.CreateIndex(
                name: "IX_visits_client_id",
                table: "visits",
                column: "client_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visits");

            migrationBuilder.DropCheckConstraint(
                name: "ck_clients_status",
                table: "clients");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "clients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "active");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "registration_date",
                table: "clients",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValueSql: "CURRENT_DATE");
        }
    }
}
