using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Logic.Migrations
{
    /// <inheritdoc />
    public partial class MigratedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WatchDurationSec",
                table: "UserReelInteractions");

            migrationBuilder.DropColumn(
                name: "AvgWatchTimeSec",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "TotalWatchTimeSec",
                table: "UserProfiles",
                newName: "TotalWatchTimeSeconds");

            migrationBuilder.AlterColumn<decimal>(
                name: "WatchPercentage",
                table: "UserReelInteractions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "WatchDurationSeconds",
                table: "UserReelInteractions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "LikeToViewRatio",
                table: "UserProfiles",
                type: "decimal(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageWatchTimeSeconds",
                table: "UserProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "UserMoviePreferences",
                type: "decimal(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "FeatureDurationSeconds",
                table: "Reels",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "DurationSeconds",
                table: "MusicTracks",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WatchDurationSeconds",
                table: "UserReelInteractions");

            migrationBuilder.DropColumn(
                name: "AverageWatchTimeSeconds",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "TotalWatchTimeSeconds",
                table: "UserProfiles",
                newName: "TotalWatchTimeSec");

            migrationBuilder.AlterColumn<double>(
                name: "WatchPercentage",
                table: "UserReelInteractions",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "WatchDurationSec",
                table: "UserReelInteractions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "LikeToViewRatio",
                table: "UserProfiles",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)",
                oldPrecision: 8,
                oldScale: 4);

            migrationBuilder.AddColumn<double>(
                name: "AvgWatchTimeSec",
                table: "UserProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "UserMoviePreferences",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)",
                oldPrecision: 8,
                oldScale: 4);

            migrationBuilder.AlterColumn<double>(
                name: "FeatureDurationSeconds",
                table: "Reels",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "DurationSeconds",
                table: "MusicTracks",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
