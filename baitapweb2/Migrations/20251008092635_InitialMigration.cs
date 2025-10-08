using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baitapweb2.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId1",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Publisher");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublisherId1",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublisherId1",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "PublisherId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Publishers");

            migrationBuilder.AddColumn<int>(
                name: "PublisherId1",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Publisher",
                columns: table => new
                {
                    PublisherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publisher", x => x.PublisherId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId1",
                table: "Books",
                column: "PublisherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publisher",
                principalColumn: "PublisherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId1",
                table: "Books",
                column: "PublisherId1",
                principalTable: "Publishers",
                principalColumn: "PublisherId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
