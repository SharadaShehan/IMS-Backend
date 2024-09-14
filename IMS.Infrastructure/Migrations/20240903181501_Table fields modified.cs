using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tablefieldsmodified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Equipments_Labs_LabId", table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_Equipments_RequstedEquipmentId",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_Items_AsignedItemId",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_users_BorrowedFrom",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_users_ReservedBy",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_users_ResponseedBy",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_users_ReturnedTo",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Equipments_EquipmentId",
                table: "Items"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Items_ItemId",
                table: "Maintenances"
            );

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

            migrationBuilder.DropPrimaryKey(name: "PK_Maintenances", table: "Maintenances");

            migrationBuilder.DropPrimaryKey(name: "PK_Labs", table: "Labs");

            migrationBuilder.DropPrimaryKey(name: "PK_Items", table: "Items");

            migrationBuilder.DropPrimaryKey(name: "PK_ItemReservations", table: "ItemReservations");

            migrationBuilder.DropIndex(
                name: "IX_ItemReservations_AsignedItemId",
                table: "ItemReservations"
            );

            migrationBuilder.DropIndex(
                name: "IX_ItemReservations_BorrowedFrom",
                table: "ItemReservations"
            );

            migrationBuilder.DropIndex(
                name: "IX_ItemReservations_RequstedEquipmentId",
                table: "ItemReservations"
            );

            migrationBuilder.DropIndex(
                name: "IX_ItemReservations_ReservedBy",
                table: "ItemReservations"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_Equipments", table: "Equipments");

            migrationBuilder.DropColumn(name: "AsignedItemId", table: "ItemReservations");

            migrationBuilder.DropColumn(name: "BorrowedFrom", table: "ItemReservations");

            migrationBuilder.DropColumn(name: "RequstedEquipmentId", table: "ItemReservations");

            migrationBuilder.DropColumn(name: "ReservedBy", table: "ItemReservations");

            migrationBuilder.RenameTable(name: "Maintenances", newName: "maintenances");

            migrationBuilder.RenameTable(name: "Labs", newName: "labs");

            migrationBuilder.RenameTable(name: "Items", newName: "items");

            migrationBuilder.RenameTable(name: "ItemReservations", newName: "itemReservations");

            migrationBuilder.RenameTable(name: "Equipments", newName: "equipments");

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_TechnicianId",
                table: "maintenances",
                newName: "IX_maintenances_TechnicianId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_ReviewedClerkId",
                table: "maintenances",
                newName: "IX_maintenances_ReviewedClerkId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_ItemId",
                table: "maintenances",
                newName: "IX_maintenances_ItemId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Maintenances_CreatedClerkId",
                table: "maintenances",
                newName: "IX_maintenances_CreatedClerkId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Items_EquipmentId",
                table: "items",
                newName: "IX_items_EquipmentId"
            );

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "itemReservations",
                newName: "StartDate"
            );

            migrationBuilder.RenameColumn(
                name: "ReturnedTo",
                table: "itemReservations",
                newName: "ReservedUserId"
            );

            migrationBuilder.RenameColumn(
                name: "ResponseedBy",
                table: "itemReservations",
                newName: "EquipmentId"
            );

            migrationBuilder.RenameColumn(
                name: "FromDate",
                table: "itemReservations",
                newName: "EndDate"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ItemReservations_ReturnedTo",
                table: "itemReservations",
                newName: "IX_itemReservations_ReservedUserId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ItemReservations_ResponseedBy",
                table: "itemReservations",
                newName: "IX_itemReservations_EquipmentId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_LabId",
                table: "equipments",
                newName: "IX_equipments_LabId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "LabName",
                table: "labs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "LabCode",
                table: "labs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedAt",
                table: "itemReservations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponedAtAt",
                table: "itemReservations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledAt",
                table: "itemReservations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "BorrowedAt",
                table: "itemReservations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "itemReservations",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "LentClerkId",
                table: "itemReservations",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "RespondedClerkId",
                table: "itemReservations",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "ReturnAcceptedClerkId",
                table: "itemReservations",
                type: "int",
                nullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "MaintenanceInterval",
                table: "equipments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_maintenances",
                table: "maintenances",
                column: "MaintenanceId"
            );

            migrationBuilder.AddPrimaryKey(name: "PK_labs", table: "labs", column: "LabId");

            migrationBuilder.AddPrimaryKey(name: "PK_items", table: "items", column: "ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_itemReservations",
                table: "itemReservations",
                column: "ItemReservationId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_equipments",
                table: "equipments",
                column: "EquipmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_itemReservations_ItemId",
                table: "itemReservations",
                column: "ItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_itemReservations_LentClerkId",
                table: "itemReservations",
                column: "LentClerkId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_itemReservations_RespondedClerkId",
                table: "itemReservations",
                column: "RespondedClerkId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_itemReservations_ReturnAcceptedClerkId",
                table: "itemReservations",
                column: "ReturnAcceptedClerkId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_equipments_labs_LabId",
                table: "equipments",
                column: "LabId",
                principalTable: "labs",
                principalColumn: "LabId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_itemReservations_equipments_EquipmentId",
                table: "itemReservations",
                column: "EquipmentId",
                principalTable: "equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_itemReservations_items_ItemId",
                table: "itemReservations",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_itemReservations_users_LentClerkId",
                table: "itemReservations",
                column: "LentClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_itemReservations_users_ReservedUserId",
                table: "itemReservations",
                column: "ReservedUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_itemReservations_users_RespondedClerkId",
                table: "itemReservations",
                column: "RespondedClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_itemReservations_users_ReturnAcceptedClerkId",
                table: "itemReservations",
                column: "ReturnAcceptedClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_items_equipments_EquipmentId",
                table: "items",
                column: "EquipmentId",
                principalTable: "equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_maintenances_items_ItemId",
                table: "maintenances",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_maintenances_users_CreatedClerkId",
                table: "maintenances",
                column: "CreatedClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_maintenances_users_ReviewedClerkId",
                table: "maintenances",
                column: "ReviewedClerkId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_maintenances_users_TechnicianId",
                table: "maintenances",
                column: "TechnicianId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_equipments_labs_LabId", table: "equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_itemReservations_equipments_EquipmentId",
                table: "itemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_itemReservations_items_ItemId",
                table: "itemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_itemReservations_users_LentClerkId",
                table: "itemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_itemReservations_users_ReservedUserId",
                table: "itemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_itemReservations_users_RespondedClerkId",
                table: "itemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_itemReservations_users_ReturnAcceptedClerkId",
                table: "itemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_items_equipments_EquipmentId",
                table: "items"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_maintenances_items_ItemId",
                table: "maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_maintenances_users_CreatedClerkId",
                table: "maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_maintenances_users_ReviewedClerkId",
                table: "maintenances"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_maintenances_users_TechnicianId",
                table: "maintenances"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_maintenances", table: "maintenances");

            migrationBuilder.DropPrimaryKey(name: "PK_labs", table: "labs");

            migrationBuilder.DropPrimaryKey(name: "PK_items", table: "items");

            migrationBuilder.DropPrimaryKey(name: "PK_itemReservations", table: "itemReservations");

            migrationBuilder.DropIndex(
                name: "IX_itemReservations_ItemId",
                table: "itemReservations"
            );

            migrationBuilder.DropIndex(
                name: "IX_itemReservations_LentClerkId",
                table: "itemReservations"
            );

            migrationBuilder.DropIndex(
                name: "IX_itemReservations_RespondedClerkId",
                table: "itemReservations"
            );

            migrationBuilder.DropIndex(
                name: "IX_itemReservations_ReturnAcceptedClerkId",
                table: "itemReservations"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_equipments", table: "equipments");

            migrationBuilder.DropColumn(name: "ItemId", table: "itemReservations");

            migrationBuilder.DropColumn(name: "LentClerkId", table: "itemReservations");

            migrationBuilder.DropColumn(name: "RespondedClerkId", table: "itemReservations");

            migrationBuilder.DropColumn(name: "ReturnAcceptedClerkId", table: "itemReservations");

            migrationBuilder.RenameTable(name: "maintenances", newName: "Maintenances");

            migrationBuilder.RenameTable(name: "labs", newName: "Labs");

            migrationBuilder.RenameTable(name: "items", newName: "Items");

            migrationBuilder.RenameTable(name: "itemReservations", newName: "ItemReservations");

            migrationBuilder.RenameTable(name: "equipments", newName: "Equipments");

            migrationBuilder.RenameIndex(
                name: "IX_maintenances_TechnicianId",
                table: "Maintenances",
                newName: "IX_Maintenances_TechnicianId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_maintenances_ReviewedClerkId",
                table: "Maintenances",
                newName: "IX_Maintenances_ReviewedClerkId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_maintenances_ItemId",
                table: "Maintenances",
                newName: "IX_Maintenances_ItemId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_maintenances_CreatedClerkId",
                table: "Maintenances",
                newName: "IX_Maintenances_CreatedClerkId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_items_EquipmentId",
                table: "Items",
                newName: "IX_Items_EquipmentId"
            );

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "ItemReservations",
                newName: "ToDate"
            );

            migrationBuilder.RenameColumn(
                name: "ReservedUserId",
                table: "ItemReservations",
                newName: "ReturnedTo"
            );

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "ItemReservations",
                newName: "ResponseedBy"
            );

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "ItemReservations",
                newName: "FromDate"
            );

            migrationBuilder.RenameIndex(
                name: "IX_itemReservations_ReservedUserId",
                table: "ItemReservations",
                newName: "IX_ItemReservations_ReturnedTo"
            );

            migrationBuilder.RenameIndex(
                name: "IX_itemReservations_EquipmentId",
                table: "ItemReservations",
                newName: "IX_ItemReservations_ResponseedBy"
            );

            migrationBuilder.RenameIndex(
                name: "IX_equipments_LabId",
                table: "Equipments",
                newName: "IX_Equipments_LabId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "LabName",
                table: "Labs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<int>(
                name: "LabCode",
                table: "Labs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<int>(
                name: "SerialNumber",
                table: "Items",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedAt",
                table: "ItemReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponedAtAt",
                table: "ItemReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledAt",
                table: "ItemReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "BorrowedAt",
                table: "ItemReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AsignedItemId",
                table: "ItemReservations",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "BorrowedFrom",
                table: "ItemReservations",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "RequstedEquipmentId",
                table: "ItemReservations",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "ReservedBy",
                table: "ItemReservations",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "MaintenanceInterval",
                table: "Equipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Maintenances",
                table: "Maintenances",
                column: "MaintenanceId"
            );

            migrationBuilder.AddPrimaryKey(name: "PK_Labs", table: "Labs", column: "LabId");

            migrationBuilder.AddPrimaryKey(name: "PK_Items", table: "Items", column: "ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemReservations",
                table: "ItemReservations",
                column: "ItemReservationId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "EquipmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ItemReservations_AsignedItemId",
                table: "ItemReservations",
                column: "AsignedItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ItemReservations_BorrowedFrom",
                table: "ItemReservations",
                column: "BorrowedFrom"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ItemReservations_RequstedEquipmentId",
                table: "ItemReservations",
                column: "RequstedEquipmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ItemReservations_ReservedBy",
                table: "ItemReservations",
                column: "ReservedBy"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Labs_LabId",
                table: "Equipments",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "LabId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_Equipments_RequstedEquipmentId",
                table: "ItemReservations",
                column: "RequstedEquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_Items_AsignedItemId",
                table: "ItemReservations",
                column: "AsignedItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_users_BorrowedFrom",
                table: "ItemReservations",
                column: "BorrowedFrom",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_users_ReservedBy",
                table: "ItemReservations",
                column: "ReservedBy",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_users_ResponseedBy",
                table: "ItemReservations",
                column: "ResponseedBy",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_users_ReturnedTo",
                table: "ItemReservations",
                column: "ReturnedTo",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Equipments_EquipmentId",
                table: "Items",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Items_ItemId",
                table: "Maintenances",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade
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
    }
}
