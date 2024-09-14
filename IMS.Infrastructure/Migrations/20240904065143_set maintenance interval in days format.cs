using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setmaintenanceintervalindaysformat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "MaintenanceInterval", table: "equipments");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceIntervalDays",
                table: "equipments",
                type: "int",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "MaintenanceIntervalDays", table: "equipments");

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceInterval",
                table: "equipments",
                type: "datetime2",
                nullable: true
            );
        }
    }
}
