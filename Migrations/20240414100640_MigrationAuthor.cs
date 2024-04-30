using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLibrary.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfBooks",
                table: "Authors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfBooks",
                table: "Authors");
        }
    }
}
