using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class MigrateCaseEnvironmentToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 기본 환경 시딩 (중복 방지)
            migrationBuilder.Sql("INSERT INTO Environments (Name,Color,SortOrder,CreatedAt) SELECT 'Dev','#4CAF50',0,datetime('now') WHERE NOT EXISTS(SELECT 1 FROM Environments WHERE Name='Dev');");
            migrationBuilder.Sql("INSERT INTO Environments (Name,Color,SortOrder,CreatedAt) SELECT 'QA','#FF9800',1,datetime('now') WHERE NOT EXISTS(SELECT 1 FROM Environments WHERE Name='QA');");
            migrationBuilder.Sql("INSERT INTO Environments (Name,Color,SortOrder,CreatedAt) SELECT 'Staging','#2196F3',2,datetime('now') WHERE NOT EXISTS(SELECT 1 FROM Environments WHERE Name='Staging');");
            migrationBuilder.Sql("INSERT INTO Environments (Name,Color,SortOrder,CreatedAt) SELECT 'Production','#F44336',3,datetime('now') WHERE NOT EXISTS(SELECT 1 FROM Environments WHERE Name='Production');");

            // 기존 Cases.Environment(int enum) → Cases.EnvironmentId(FK) 매핑
            migrationBuilder.Sql(@"
                UPDATE Cases SET EnvironmentId = (
                    SELECT Id FROM Environments WHERE Name = CASE Cases.Environment
                        WHEN 0 THEN 'Dev'
                        WHEN 1 THEN 'QA'
                        WHEN 2 THEN 'Staging'
                        WHEN 3 THEN 'Production'
                    END
                ) WHERE EnvironmentId IS NULL AND Environment IN (0,1,2,3);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
