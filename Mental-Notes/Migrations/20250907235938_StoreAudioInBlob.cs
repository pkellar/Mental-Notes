using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalNotes.Migrations
{
    /// <inheritdoc />
    public partial class StoreAudioInBlob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioFile",
                table: "Episodes");

            migrationBuilder.AddColumn<string>(
                name: "AudioBlobUrl",
                table: "Episodes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioBlobUrl",
                table: "Episodes");

            migrationBuilder.AddColumn<byte[]>(
                name: "AudioFile",
                table: "Episodes",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
