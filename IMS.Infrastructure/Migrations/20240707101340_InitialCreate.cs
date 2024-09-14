using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Labs",
                columns: table => new
                {
                    LabId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabCode = table.Column<int>(type: "int", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labs", x => x.LabId);
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    EquipmentId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabId = table.Column<int>(type: "int", nullable: false),
                    MaintenanceInterval = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipments_Labs_LabId",
                        column: x => x.LabId,
                        principalTable: "Labs",
                        principalColumn: "LabId",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ItemReservations",
                columns: table => new
                {
                    ItemReservationId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequstedEquipmentId = table.Column<int>(type: "int", nullable: false),
                    AsignedItemId = table.Column<int>(type: "int", nullable: false),
                    ReservedBy = table.Column<int>(type: "int", nullable: false),
                    ResponseedBy = table.Column<int>(type: "int", nullable: false),
                    BorrowedFrom = table.Column<int>(type: "int", nullable: false),
                    ReturnedTo = table.Column<int>(type: "int", nullable: false),
                    ResponseNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponedAtAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorrowedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemReservations", x => x.ItemReservationId);
                    table.ForeignKey(
                        name: "FK_ItemReservations_Equipments_RequstedEquipmentId",
                        column: x => x.RequstedEquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ItemReservations_Items_AsignedItemId",
                        column: x => x.AsignedItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ItemReservations_users_BorrowedFrom",
                        column: x => x.BorrowedFrom,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ItemReservations_users_ReservedBy",
                        column: x => x.ReservedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ItemReservations_users_ResponseedBy",
                        column: x => x.ResponseedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_ItemReservations_users_ReturnedTo",
                        column: x => x.ReturnedTo,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Maintenances",
                columns: table => new
                {
                    MaintenanceId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    AssignedTechnician = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: false),
                    ReviwedBy = table.Column<int>(type: "int", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CeatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RepairedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenances", x => x.MaintenanceId);
                    table.ForeignKey(
                        name: "FK_Maintenances_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Maintenances_users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Maintenances_users_AssignedTechnician",
                        column: x => x.AssignedTechnician,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Maintenances_users_ReviwedBy",
                        column: x => x.ReviwedBy,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_LabId",
                table: "Equipments",
                column: "LabId"
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

            migrationBuilder.CreateIndex(
                name: "IX_ItemReservations_ResponseedBy",
                table: "ItemReservations",
                column: "ResponseedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ItemReservations_ReturnedTo",
                table: "ItemReservations",
                column: "ReturnedTo"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Items_EquipmentId",
                table: "Items",
                column: "EquipmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedBy",
                table: "Maintenances",
                column: "AssignedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedTechnician",
                table: "Maintenances",
                column: "AssignedTechnician"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_ItemId",
                table: "Maintenances",
                column: "ItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_ReviwedBy",
                table: "Maintenances",
                column: "ReviwedBy"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ItemReservations");

            migrationBuilder.DropTable(name: "Maintenances");

            migrationBuilder.DropTable(name: "Items");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.DropTable(name: "Equipments");

            migrationBuilder.DropTable(name: "Labs");
        }
    }
}
