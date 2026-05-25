using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalNotes.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureFieldToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "Books",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Books");
        }
    }
}
