using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetroManager_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerNotesToRetrospective : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerNotes",
                table: "Retrospectives",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerNotes",
                table: "Retrospectives");
        }
    }
}
