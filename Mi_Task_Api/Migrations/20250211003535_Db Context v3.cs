using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mi_Task_Api.Migrations
{
    /// <inheritdoc />
    public partial class DbContextv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoredTasks_Tasks_IdTask",
                table: "ScoredTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_Id",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tasks",
                newName: "IdUser");

            migrationBuilder.AlterColumn<int>(
                name: "IdTask",
                table: "ScoredTasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_IdUser",
                table: "Tasks",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoredTasks_Tasks_IdTask",
                table: "ScoredTasks",
                column: "IdTask",
                principalTable: "Tasks",
                principalColumn: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_IdUser",
                table: "Tasks",
                column: "IdUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoredTasks_Tasks_IdTask",
                table: "ScoredTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_IdUser",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_IdUser",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "IdUser",
                table: "Tasks",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "IdTask",
                table: "ScoredTasks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoredTasks_Tasks_IdTask",
                table: "ScoredTasks",
                column: "IdTask",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_Id",
                table: "Tasks",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
