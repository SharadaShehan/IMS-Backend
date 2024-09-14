using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Equipments_Labs_LabId", table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_Items_AsignedItemId",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Equipments_EquipmentId",
                table: "Items"
            );

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Specification",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: true
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
                name: "FK_ItemReservations_Items_AsignedItemId",
                table: "ItemReservations",
                column: "AsignedItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Equipments_EquipmentId",
                table: "Items",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Equipments_Labs_LabId", table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemReservations_Items_AsignedItemId",
                table: "ItemReservations"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Equipments_EquipmentId",
                table: "Items"
            );

            migrationBuilder.DropColumn(name: "Model", table: "Equipments");

            migrationBuilder.DropColumn(name: "Specification", table: "Equipments");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Labs_LabId",
                table: "Equipments",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "LabId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ItemReservations_Items_AsignedItemId",
                table: "ItemReservations",
                column: "AsignedItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Equipments_EquipmentId",
                table: "Items",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "EquipmentId",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
