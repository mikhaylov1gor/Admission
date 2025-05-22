using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdmissionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmissionSettings",
                columns: table => new
                {
                    AdmissionSeasonId = table.Column<int>(type: "integer", nullable: false),
                    UniversityAdmissionStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "StudentAdmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdmissionSeasonId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdmissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentAdmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionPrograms_StudentAdmissions_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "StudentAdmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPrograms_StudentAdmissionId",
                table: "AdmissionPrograms",
                column: "StudentAdmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionPrograms");

            migrationBuilder.DropTable(
                name: "AdmissionSettings");

            migrationBuilder.DropTable(
                name: "StudentAdmissions");
        }
    }
}
