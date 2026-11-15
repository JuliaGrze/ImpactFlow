using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImpactFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAtlas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "atlas_maps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atlas_maps", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "atlas_versions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    map_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atlas_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_atlas_versions_atlas_maps_map_id",
                        column: x => x.map_id,
                        principalTable: "atlas_maps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_atlas_versions_map_id_number",
                table: "atlas_versions",
                columns: new[] { "map_id", "number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atlas_versions");

            migrationBuilder.DropTable(
                name: "atlas_maps");
        }
    }
}
