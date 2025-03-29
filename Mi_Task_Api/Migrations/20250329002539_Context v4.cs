using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mi_Task_Api.Migrations
{
    /// <inheritdoc />
    public partial class Contextv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "ScoredTasks",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "ScoredTasks");
        }
    }
}
