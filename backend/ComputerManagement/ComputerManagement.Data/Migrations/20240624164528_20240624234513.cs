using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class _20240624234513 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_computer_history_MonitorSessionId",
                table: "computer_history",
                column: "MonitorSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_computer_history_monitor_session_MonitorSessionId",
                table: "computer_history",
                column: "MonitorSessionId",
                principalTable: "monitor_session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_computer_history_monitor_session_MonitorSessionId",
                table: "computer_history");

            migrationBuilder.DropIndex(
                name: "IX_computer_history_MonitorSessionId",
                table: "computer_history");
        }
    }
}
