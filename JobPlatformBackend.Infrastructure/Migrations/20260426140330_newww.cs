using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPlatformBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newww : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoadmapJson",
                table: "CareerArchitects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoadmapJson",
                table: "CareerArchitects");
        }
    }
}
