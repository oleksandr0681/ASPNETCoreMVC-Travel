using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel.Migrations
{
    /// <inheritdoc />
    public partial class PropertiesChange1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Places_PlaceId",
                table: "Marks");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Places",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "Marks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Places_ApplicationUserId",
                table: "Places",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_Places_PlaceId",
                table: "Marks",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Places_AspNetUsers_ApplicationUserId",
                table: "Places",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Places_PlaceId",
                table: "Marks");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_AspNetUsers_ApplicationUserId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Places_ApplicationUserId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Places");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "Marks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_Places_PlaceId",
                table: "Marks",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id");
        }
    }
}
