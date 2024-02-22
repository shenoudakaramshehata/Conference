using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conference.Migrations
{
    /// <inheritdoc />
    public partial class InitialConferenceContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EventLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventBarcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EventIsActive = table.Column<bool>(type: "bit", nullable: true),
                    EventTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventVideo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "EventActivity_Single",
                columns: table => new
                {
                    EventActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventActivityTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventActivityDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventActivityType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventActivity_Single", x => x.EventActivityId);
                    table.ForeignKey(
                        name: "FK_EventActivity_Single_Event",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSubscription",
                columns: table => new
                {
                    EventSubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventSubscriptionType = table.Column<int>(type: "int", nullable: true),
                    EventSubscriptionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EventSubscriptionQueue = table.Column<int>(type: "int", nullable: true),
                    EventSubscriptionFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventSubscriptionMobile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSubscription", x => x.EventSubscriptionId);
                    table.ForeignKey(
                        name: "FK_EventSubscription_Event",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionActivity",
                columns: table => new
                {
                    SubscriptionActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    EventActivityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionActivity", x => x.SubscriptionActivityId);
                    table.ForeignKey(
                        name: "FK_SubscriptionActivity_EventActivity_Single",
                        column: x => x.EventActivityId,
                        principalTable: "EventActivity_Single",
                        principalColumn: "EventActivityId");
                    table.ForeignKey(
                        name: "FK_SubscriptionActivity_EventSubscription",
                        column: x => x.EventSubscriptionId,
                        principalTable: "EventSubscription",
                        principalColumn: "EventSubscriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventActivity_Single_EventId",
                table: "EventActivity_Single",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubscription_EventId",
                table: "EventSubscription",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionActivity_EventActivityId",
                table: "SubscriptionActivity",
                column: "EventActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionActivity_EventSubscriptionId",
                table: "SubscriptionActivity",
                column: "EventSubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionActivity");

            migrationBuilder.DropTable(
                name: "EventActivity_Single");

            migrationBuilder.DropTable(
                name: "EventSubscription");

            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}
