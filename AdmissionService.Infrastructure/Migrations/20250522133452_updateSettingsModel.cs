using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdmissionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateSettingsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AdmissionSettings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdmissionSettings",
                table: "AdmissionSettings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdmissionSettings",
                table: "AdmissionSettings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AdmissionSettings");
        }
    }
}
