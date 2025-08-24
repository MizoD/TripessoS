using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editSomeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Hotel_HotelId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Countries_CountryId",
                table: "Hotel");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Trips_TripId",
                table: "Hotel");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelCarts_Hotel_HotelId",
                table: "HotelCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelWishlists_Hotel_HotelId",
                table: "HotelWishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Bookings_BookingId",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Bookings_BookingId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Seats_SeatId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Seats_BookingId",
                table: "Seats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_SeatId",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hotel",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FlightCarts");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "Ticket");

            migrationBuilder.RenameTable(
                name: "Ticket",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "Hotel",
                newName: "Hotels");

            migrationBuilder.RenameColumn(
                name: "NumberOfTickets",
                table: "Bookings",
                newName: "Tickets");

            migrationBuilder.RenameColumn(
                name: "RegistraionDate",
                table: "AspNetUsers",
                newName: "RegistrationDate");

            migrationBuilder.RenameColumn(
                name: "OTPNumber",
                table: "ApplicationUserOTPs",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_BookingId",
                table: "Tickets",
                newName: "IX_Tickets_BookingId");

            migrationBuilder.RenameColumn(
                name: "AvilableRooms",
                table: "Hotels",
                newName: "Traffic");

            migrationBuilder.RenameIndex(
                name: "IX_Hotel_TripId",
                table: "Hotels",
                newName: "IX_Hotels_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Hotel_CountryId",
                table: "Hotels",
                newName: "IX_Hotels_CountryId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "Trips",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Trips",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "Traffic",
                table: "Flights",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TripId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FlightId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "AvailableRooms",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hotels",
                table: "Hotels",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passengers_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passengers_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_BookingId",
                table: "Passengers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_SeatId",
                table: "Passengers",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Hotels_HotelId",
                table: "Bookings",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelCarts_Hotels_HotelId",
                table: "HotelCarts",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Countries_CountryId",
                table: "Hotels",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Trips_TripId",
                table: "Hotels",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelWishlists_Hotels_HotelId",
                table: "HotelWishlists",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Bookings_BookingId",
                table: "Tickets",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Hotels_HotelId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelCarts_Hotels_HotelId",
                table: "HotelCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Countries_CountryId",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Trips_TripId",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelWishlists_Hotels_HotelId",
                table: "HotelWishlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Bookings_BookingId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hotels",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Traffic",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AvailableRooms",
                table: "Hotels");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Ticket");

            migrationBuilder.RenameTable(
                name: "Hotels",
                newName: "Hotel");

            migrationBuilder.RenameColumn(
                name: "Tickets",
                table: "Bookings",
                newName: "NumberOfTickets");

            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "AspNetUsers",
                newName: "RegistraionDate");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "ApplicationUserOTPs",
                newName: "OTPNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_BookingId",
                table: "Ticket",
                newName: "IX_Ticket_BookingId");

            migrationBuilder.RenameColumn(
                name: "Traffic",
                table: "Hotel",
                newName: "AvilableRooms");

            migrationBuilder.RenameIndex(
                name: "IX_Hotels_TripId",
                table: "Hotel",
                newName: "IX_Hotel_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Hotels_CountryId",
                table: "Hotel",
                newName: "IX_Hotel_CountryId");

            migrationBuilder.AlterColumn<double>(
                name: "Rate",
                table: "Trips",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Trips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FlightCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TripId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FlightId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "Ticket",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeatId",
                table: "Ticket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hotel",
                table: "Hotel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_BookingId",
                table: "Seats",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SeatId",
                table: "Ticket",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Hotel_HotelId",
                table: "Bookings",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Countries_CountryId",
                table: "Hotel",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Trips_TripId",
                table: "Hotel",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelCarts_Hotel_HotelId",
                table: "HotelCarts",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HotelWishlists_Hotel_HotelId",
                table: "HotelWishlists",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Bookings_BookingId",
                table: "Seats",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Bookings_BookingId",
                table: "Ticket",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Seats_SeatId",
                table: "Ticket",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
