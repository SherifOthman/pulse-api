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
            // 1. Create the join table only if it doesn't already exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DoctorSpecializations')
                BEGIN
                    CREATE TABLE [DoctorSpecializations] (
                        [DoctorProfileId] uniqueidentifier NOT NULL,
                        [SpecializationId] uniqueidentifier NOT NULL,
                        CONSTRAINT [PK_DoctorSpecializations] PRIMARY KEY ([DoctorProfileId], [SpecializationId]),
                        CONSTRAINT [FK_DoctorSpecializations_DoctorProfiles_DoctorProfileId]
                            FOREIGN KEY ([DoctorProfileId]) REFERENCES [DoctorProfiles] ([BusinessId]) ON DELETE CASCADE,
                        CONSTRAINT [FK_DoctorSpecializations_Specializations_SpecializationId]
                            FOREIGN KEY ([SpecializationId]) REFERENCES [Specializations] ([Id]) ON DELETE NO ACTION
                    );
                    CREATE INDEX [IX_DoctorSpecializations_SpecializationId]
                        ON [DoctorSpecializations] ([SpecializationId]);
                END
            ");

            // 2. Migrate existing data — only if the source column still exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'DoctorProfiles' AND COLUMN_NAME = 'SpecializationId'
                )
                BEGIN
                    INSERT INTO DoctorSpecializations (DoctorProfileId, SpecializationId)
                    SELECT dp.BusinessId, dp.SpecializationId
                    FROM DoctorProfiles dp
                    WHERE dp.SpecializationId != '00000000-0000-0000-0000-000000000000'
                      AND EXISTS (SELECT 1 FROM Specializations s WHERE s.Id = dp.SpecializationId)
                      AND NOT EXISTS (
                          SELECT 1 FROM DoctorSpecializations ds
                          WHERE ds.DoctorProfileId = dp.BusinessId AND ds.SpecializationId = dp.SpecializationId
                      );
                END
            ");

            // 3. Drop the old FK and column — only if they still exist
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'DoctorProfiles' AND COLUMN_NAME = 'SpecializationId'
                )
                BEGIN
                    DECLARE @fkName NVARCHAR(256);
                    SELECT @fkName = fk.name
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
                    WHERE t.name = 'DoctorProfiles' AND c.name = 'SpecializationId';

                    IF @fkName IS NOT NULL
                        EXEC('ALTER TABLE [DoctorProfiles] DROP CONSTRAINT [' + @fkName + ']');

                    ALTER TABLE [DoctorProfiles] DROP COLUMN [SpecializationId];
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore SpecializationId column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'DoctorProfiles' AND COLUMN_NAME = 'SpecializationId'
                )
                BEGIN
                    ALTER TABLE [DoctorProfiles] ADD [SpecializationId] uniqueidentifier NOT NULL
                        CONSTRAINT [DF_DoctorProfiles_SpecializationId] DEFAULT '00000000-0000-0000-0000-000000000000';

                    -- Restore data from join table (first specialization per doctor)
                    UPDATE dp
                    SET dp.SpecializationId = (
                        SELECT TOP 1 ds.SpecializationId
                        FROM DoctorSpecializations ds
                        WHERE ds.DoctorProfileId = dp.BusinessId
                    )
                    FROM DoctorProfiles dp
                    WHERE EXISTS (
                        SELECT 1 FROM DoctorSpecializations ds WHERE ds.DoctorProfileId = dp.BusinessId
                    );

                    ALTER TABLE [DoctorProfiles] ADD CONSTRAINT [FK_DoctorProfiles_Specializations_SpecializationId]
                        FOREIGN KEY ([SpecializationId]) REFERENCES [Specializations] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.DropTable(name: "DoctorSpecializations");
        }
    }
}
