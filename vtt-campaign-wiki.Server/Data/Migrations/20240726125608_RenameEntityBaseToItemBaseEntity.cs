using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vtt_campaign_wiki.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameEntityBaseToItemBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignPlayerEntity_EntityBase_CampaignId",
                table: "CampaignPlayerEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityBase_AspNetUsers_AuthorId",
                table: "EntityBase");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityBase_EntityBase_CampaignId",
                table: "EntityBase");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityBase_EntityBase_CampaignItemEntity_CampaignId",
                table: "EntityBase");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityBase_EntityBase_ParentEntityId",
                table: "EntityBase");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityBase_Images_ImageId",
                table: "EntityBase");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionPlayerEntity_EntityBase_SessionId",
                table: "SessionPlayerEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityBase",
                table: "EntityBase");

            migrationBuilder.RenameTable(
                name: "EntityBase",
                newName: "ItemBaseEntity");

            migrationBuilder.RenameIndex(
                name: "IX_EntityBase_ParentEntityId",
                table: "ItemBaseEntity",
                newName: "IX_ItemBaseEntity_ParentEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityBase_ImageId",
                table: "ItemBaseEntity",
                newName: "IX_ItemBaseEntity_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityBase_CampaignItemEntity_CampaignId",
                table: "ItemBaseEntity",
                newName: "IX_ItemBaseEntity_CampaignItemEntity_CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityBase_CampaignId",
                table: "ItemBaseEntity",
                newName: "IX_ItemBaseEntity_CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityBase_AuthorId",
                table: "ItemBaseEntity",
                newName: "IX_ItemBaseEntity_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemBaseEntity",
                table: "ItemBaseEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignPlayerEntity_ItemBaseEntity_CampaignId",
                table: "CampaignPlayerEntity",
                column: "CampaignId",
                principalTable: "ItemBaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBaseEntity_AspNetUsers_AuthorId",
                table: "ItemBaseEntity",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBaseEntity_Images_ImageId",
                table: "ItemBaseEntity",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBaseEntity_ItemBaseEntity_CampaignId",
                table: "ItemBaseEntity",
                column: "CampaignId",
                principalTable: "ItemBaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBaseEntity_ItemBaseEntity_CampaignItemEntity_CampaignId",
                table: "ItemBaseEntity",
                column: "CampaignItemEntity_CampaignId",
                principalTable: "ItemBaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBaseEntity_ItemBaseEntity_ParentEntityId",
                table: "ItemBaseEntity",
                column: "ParentEntityId",
                principalTable: "ItemBaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionPlayerEntity_ItemBaseEntity_SessionId",
                table: "SessionPlayerEntity",
                column: "SessionId",
                principalTable: "ItemBaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignPlayerEntity_ItemBaseEntity_CampaignId",
                table: "CampaignPlayerEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBaseEntity_AspNetUsers_AuthorId",
                table: "ItemBaseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBaseEntity_Images_ImageId",
                table: "ItemBaseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBaseEntity_ItemBaseEntity_CampaignId",
                table: "ItemBaseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBaseEntity_ItemBaseEntity_CampaignItemEntity_CampaignId",
                table: "ItemBaseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemBaseEntity_ItemBaseEntity_ParentEntityId",
                table: "ItemBaseEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionPlayerEntity_ItemBaseEntity_SessionId",
                table: "SessionPlayerEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemBaseEntity",
                table: "ItemBaseEntity");

            migrationBuilder.RenameTable(
                name: "ItemBaseEntity",
                newName: "EntityBase");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBaseEntity_ParentEntityId",
                table: "EntityBase",
                newName: "IX_EntityBase_ParentEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBaseEntity_ImageId",
                table: "EntityBase",
                newName: "IX_EntityBase_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBaseEntity_CampaignItemEntity_CampaignId",
                table: "EntityBase",
                newName: "IX_EntityBase_CampaignItemEntity_CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBaseEntity_CampaignId",
                table: "EntityBase",
                newName: "IX_EntityBase_CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemBaseEntity_AuthorId",
                table: "EntityBase",
                newName: "IX_EntityBase_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityBase",
                table: "EntityBase",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignPlayerEntity_EntityBase_CampaignId",
                table: "CampaignPlayerEntity",
                column: "CampaignId",
                principalTable: "EntityBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityBase_AspNetUsers_AuthorId",
                table: "EntityBase",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityBase_EntityBase_CampaignId",
                table: "EntityBase",
                column: "CampaignId",
                principalTable: "EntityBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityBase_EntityBase_CampaignItemEntity_CampaignId",
                table: "EntityBase",
                column: "CampaignItemEntity_CampaignId",
                principalTable: "EntityBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityBase_EntityBase_ParentEntityId",
                table: "EntityBase",
                column: "ParentEntityId",
                principalTable: "EntityBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityBase_Images_ImageId",
                table: "EntityBase",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionPlayerEntity_EntityBase_SessionId",
                table: "SessionPlayerEntity",
                column: "SessionId",
                principalTable: "EntityBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
