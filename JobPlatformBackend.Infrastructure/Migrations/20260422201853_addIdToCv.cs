using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPlatformBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIdToCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CvPublicId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvPublicId",
                table: "Applications");
        }
    }
}
