using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPlatformBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Companies",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //        Descriptoin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Companies", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Skills",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Skills", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Jobs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //        Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
            //        Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            //        TypeJop = table.Column<int>(type: "int", nullable: false),
            //        ExperieceLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        CompanyId = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Jobs", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Jobs_Companies_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Companies",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        HashPassword = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        Role = table.Column<int>(type: "int", nullable: false),
            //        Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        ProfileImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //        Headline = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            //        Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
            //        About = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            //        CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //        EmailVerificationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VerificationCodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
            //        CompanyId = table.Column<int>(type: "int", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Users_Companies_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Companies",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "JobSkills",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        JobId = table.Column<int>(type: "int", nullable: false),
            //        SkillId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_JobSkills", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_JobSkills_Jobs_JobId",
            //            column: x => x.JobId,
            //            principalTable: "Jobs",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_JobSkills_Skills_SkillId",
            //            column: x => x.SkillId,
            //            principalTable: "Skills",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Applications",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        JobId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CvUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Applications", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Applications_Jobs_JobId",
            //            column: x => x.JobId,
            //            principalTable: "Jobs",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Applications_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CompanyAdmins",
            //    columns: table => new
            //    {
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CompanyId = table.Column<int>(type: "int", nullable: false),
            //        AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Id = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CompanyAdmins", x => new { x.UserId, x.CompanyId });
            //        table.ForeignKey(
            //            name: "FK_CompanyAdmins_Companies_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Companies",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_CompanyAdmins_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Posts",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Posts", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Posts_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserRefreshToken",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsRevoked = table.Column<bool>(type: "bit", nullable: false),
            //        ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserRefreshToken", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserRefreshToken_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserSkills",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        SkillId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserSkills", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserSkills_Skills_SkillId",
            //            column: x => x.SkillId,
            //            principalTable: "Skills",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UserSkills_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PostComments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
            //        PostId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PostComments", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PostComments_Posts_PostId",
            //            column: x => x.PostId,
            //            principalTable: "Posts",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PostComments_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PostLikes",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PostId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PostLikes", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PostLikes_Posts_PostId",
            //            column: x => x.PostId,
            //            principalTable: "Posts",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_PostLikes_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Applications_JobId",
            //    table: "Applications",
            //    column: "JobId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Applications_UserId",
            //    table: "Applications",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CompanyAdmins_CompanyId",
            //    table: "CompanyAdmins",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Jobs_CompanyId",
            //    table: "Jobs",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_JobSkills_JobId_SkillId",
            //    table: "JobSkills",
            //    columns: new[] { "JobId", "SkillId" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_JobSkills_SkillId",
            //    table: "JobSkills",
            //    column: "SkillId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PostComments_PostId",
            //    table: "PostComments",
            //    column: "PostId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PostComments_UserId",
            //    table: "PostComments",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PostLikes_PostId",
            //    table: "PostLikes",
            //    column: "PostId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PostLikes_UserId",
            //    table: "PostLikes",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Posts_UserId",
            //    table: "Posts",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRefreshToken_UserId",
            //    table: "UserRefreshToken",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_CompanyId",
            //    table: "Users",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_Email",
            //    table: "Users",
            //    column: "Email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserSkills_SkillId",
            //    table: "UserSkills",
            //    column: "SkillId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserSkills_UserID",
            //    table: "UserSkills",
            //    column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "CompanyAdmins");

            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "PostComments");

            migrationBuilder.DropTable(
                name: "PostLikes");

            migrationBuilder.DropTable(
                name: "UserRefreshToken");

            migrationBuilder.DropTable(
                name: "UserSkills");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
