using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ep_synoptic_2005.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalFileNameToUploadFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "UploadFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "UploadFiles");
        }
    }
}
