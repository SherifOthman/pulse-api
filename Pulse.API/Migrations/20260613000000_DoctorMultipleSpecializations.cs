using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.API.Migrations
{
    /// <inheritdoc />
    public partial class DoctorMultipleSpecializations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create the new join table
            migrationBuilder.CreateTable(
                name: "DoctorSpecializations",
                columns: table => new
                {
                    DoctorProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSpecializations", x => new { x.DoctorProfileId, x.SpecializationId });
                    table.ForeignKey(
                        name: "FK_DoctorSpecializations_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSpecializations_SpecializationId",
                table: "DoctorSpecializations",
                column: "SpecializationId");

            // 2. Migrate existing data: copy SpecializationId from DoctorProfiles into the join table
            //    Only copy rows where SpecializationId is not the empty GUID (doctors that had no specialization)
            migrationBuilder.Sql(@"
                INSERT INTO DoctorSpecializations (DoctorProfileId, SpecializationId)
                SELECT dp.BusinessId, dp.SpecializationId
                FROM DoctorProfiles dp
                WHERE dp.SpecializationId != '00000000-0000-0000-0000-000000000000'
                  AND EXISTS (
                      SELECT 1 FROM Specializations s WHERE s.Id = dp.SpecializationId
                  )
            ");

            // 3. Drop the old FK and column
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Specializations_SpecializationId",
                table: "DoctorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DoctorProfiles_SpecializationId",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "DoctorProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the old column
            migrationBuilder.AddColumn<Guid>(
                name: "SpecializationId",
                table: "DoctorProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Restore data: take the first specialization from the join table
            migrationBuilder.Sql(@"
                UPDATE dp
                SET dp.SpecializationId = (
                    SELECT TOP 1 ds.SpecializationId
                    FROM DoctorSpecializations ds
                    WHERE ds.DoctorProfileId = dp.BusinessId
                )
                FROM DoctorProfiles dp
                WHERE EXISTS (
                    SELECT 1 FROM DoctorSpecializations ds WHERE ds.DoctorProfileId = dp.BusinessId
                )
            ");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfiles_SpecializationId",
                table: "DoctorProfiles",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Specializations_SpecializationId",
                table: "DoctorProfiles",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropTable(name: "DoctorSpecializations");
        }
    }
}
