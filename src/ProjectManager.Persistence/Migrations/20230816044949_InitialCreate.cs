using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "projectmanager");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "projectmanager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "projectmanager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "projectmanager",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                schema: "projectmanager",
                columns: table => new
                {
                    CollaboratorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => new { x.CollaboratorsId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectUser_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "projectmanager",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Users_CollaboratorsId",
                        column: x => x.CollaboratorsId,
                        principalSchema: "projectmanager",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "projectmanager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "projectmanager",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "projectmanager",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                schema: "projectmanager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "projectmanager",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "projectmanager",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                schema: "projectmanager",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_ProjectId",
                schema: "projectmanager",
                table: "ProjectUser",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_AuthorId",
                schema: "projectmanager",
                table: "TaskComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskId",
                schema: "projectmanager",
                table: "TaskComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AuthorId",
                schema: "projectmanager",
                table: "Tasks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                schema: "projectmanager",
                table: "Tasks",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectUser",
                schema: "projectmanager");

            migrationBuilder.DropTable(
                name: "TaskComments",
                schema: "projectmanager");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "projectmanager");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "projectmanager");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "projectmanager");
        }
    }
}
