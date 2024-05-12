using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLibrary.Migrations
{
    /// <inheritdoc />
    public partial class ListBooksClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientID",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_ClientID",
                table: "Books",
                column: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Clients_ClientID",
                table: "Books",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Clients_ClientID",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_ClientID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ClientID",
                table: "Books");
        }
    }
}
