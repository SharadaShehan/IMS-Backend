using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Maintenancetableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_users_AssignedBy",
                table: "Maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_users_AssignedTechnician",
                table: "Maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_users_ReviwedBy",
                table: "Maintenances"
            );

            migrationBuilder.DropIndex(name: "IX_Maintenances_AssignedBy", table: "Maintenances");

            migrationBuilder.DropColumn(name: "AssignedBy", table: "Maintenances");

            migrationBuilder.DropColumn(name: "CeatedAt", table: "Maintenances");

            migrationBuilder.RenameColumn(
                name: "ReviewedAT",
                table: "Maintenances",
                newName: "ReviewedAt"
            );

            migrationBuilder.RenameColumn(
                name: "ReviwedBy",
                table: "Maintenances",
                newName: "TechnicianId"
            );

            migrationBuilder.RenameColumn(
                name: "ReviewedBy",
                table: "Maintenances",
                newName: "ReviewedClerkId"
            );

            migrationBuilder.RenameColumn(
                name: "RepairedAt",
                table: "Maintenances",
                newName: "CreatedAt"
            );

            migrationBuilder.RenameColumn(
                name: "AssignedTechnician",
                table: "Maintenances",
                newName: "CreatedClerkId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_ReviwedBy",
                table: "Maintenances",
                newName: "IX_Maintenances_TechnicianId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_AssignedTechnician",
                table: "Maintenances",
                newName: "IX_Maintenances_CreatedClerkId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "TaskDescription",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReviewedAt",
                table: "Maintenances",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<int>(
                name: "Cost",
                table: "Maintenances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Maintenances",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_ReviewedClerkId",
                table: "Maintenances",
                column: "ReviewedClerkId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_users_CreatedClerkId",
                table: "Maintenances",
                column: "CreatedClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_users_ReviewedClerkId",
                table: "Maintenances",
                column: "ReviewedClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_users_TechnicianId",
                table: "Maintenances",
                column: "TechnicianId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_users_CreatedClerkId",
                table: "Maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_users_ReviewedClerkId",
                table: "Maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_users_TechnicianId",
                table: "Maintenances"
            );

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_ReviewedClerkId",
                table: "Maintenances"
            );

            migrationBuilder.DropColumn(name: "SubmittedAt", table: "Maintenances");

            migrationBuilder.RenameColumn(
                name: "ReviewedAt",
                table: "Maintenances",
                newName: "ReviewedAT"
            );

            migrationBuilder.RenameColumn(
                name: "TechnicianId",
                table: "Maintenances",
                newName: "ReviwedBy"
            );

            migrationBuilder.RenameColumn(
                name: "ReviewedClerkId",
                table: "Maintenances",
                newName: "ReviewedBy"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedClerkId",
                table: "Maintenances",
                newName: "AssignedTechnician"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Maintenances",
                newName: "RepairedAt"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_TechnicianId",
                table: "Maintenances",
                newName: "IX_Maintenances_ReviwedBy"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_CreatedClerkId",
                table: "Maintenances",
                newName: "IX_Maintenances_AssignedTechnician"
            );

            migrationBuilder.AlterColumn<string>(
                name: "TaskDescription",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReviewedAT",
                table: "Maintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "Cost",
                table: "Maintenances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AssignedBy",
                table: "Maintenances",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "CeatedAt",
                table: "Maintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedBy",
                table: "Maintenances",
                column: "AssignedBy"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_users_AssignedBy",
                table: "Maintenances",
                column: "AssignedBy",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_users_AssignedTechnician",
                table: "Maintenances",
                column: "AssignedTechnician",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_users_ReviwedBy",
                table: "Maintenances",
                column: "ReviwedBy",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
