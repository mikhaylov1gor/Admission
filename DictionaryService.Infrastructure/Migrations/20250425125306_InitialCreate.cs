using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DictionaryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EducationLevelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EducationDocumentTypeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationLevels_EducationDocumentTypes_EducationDocumentTyp~",
                        column: x => x.EducationDocumentTypeId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EducationPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: false),
                    EducationForm = table.Column<string>(type: "text", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationLevelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationPrograms_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationPrograms_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypes_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_EducationDocumentTypeId",
                table: "EducationLevels",
                column: "EducationDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationPrograms_EducationLevelId",
                table: "EducationPrograms",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationPrograms_FacultyId",
                table: "EducationPrograms",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes");

            migrationBuilder.DropTable(
                name: "EducationPrograms");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropTable(
                name: "EducationDocumentTypes");
        }
    }
}
