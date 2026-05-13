using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WatchPercentage",
                table: "UserReelInteractions",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StarRating",
                table: "MovieReviews",
                type: "decimal(3,1)",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WatchPercentage",
                table: "UserReelInteractions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "StarRating",
                table: "MovieReviews",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,1)",
                oldPrecision: 3,
                oldScale: 1);
        }
    }
}
