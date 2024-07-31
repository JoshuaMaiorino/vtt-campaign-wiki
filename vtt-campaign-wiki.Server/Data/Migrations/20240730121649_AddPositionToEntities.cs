using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vtt_campaign_wiki.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Position",
                table: "ItemBaseEntity",
                type: "decimal(18,5)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ItemBaseEntity_Position",
                table: "ItemBaseEntity",
                column: "Position");

            migrationBuilder.Sql( $@"
    WITH RECURSIVE PositionUpdate AS (
        SELECT Id, {Features.Shared.Constants.ItemBase.POSITION_GAP} AS Position, 1 AS RowNumber
        FROM ItemBaseEntity
        WHERE Id = (SELECT MIN(Id) FROM ItemBaseEntity)
        UNION ALL
        SELECT i.Id, pu.Position + {Features.Shared.Constants.ItemBase.POSITION_GAP}, pu.RowNumber + 1
        FROM ItemBaseEntity i
        JOIN PositionUpdate pu ON i.Id = (SELECT MIN(Id) FROM ItemBaseEntity WHERE Id > pu.Id)
    )
    UPDATE ItemBaseEntity
    SET Position = (SELECT Position FROM PositionUpdate WHERE ItemBaseEntity.Id = PositionUpdate.Id);
" );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemBaseEntity_Position",
                table: "ItemBaseEntity");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "ItemBaseEntity");
        }
    }
}
