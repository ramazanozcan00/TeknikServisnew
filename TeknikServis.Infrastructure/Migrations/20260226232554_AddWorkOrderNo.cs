using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeknikServis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkOrderNo",
                table: "WorkOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderNo",
                table: "WorkOrders",
                column: "WorkOrderNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_WorkOrderNo",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "WorkOrderNo",
                table: "WorkOrders");
        }
    }
}
