using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEducationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "EducationLevels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "EducationLevels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
