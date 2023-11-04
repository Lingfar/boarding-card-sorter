using System;
using Domain.BoardingCards;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:boarding_card_type", "bus,train,plane")
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.CreateTable(
            name: "boarding_card",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                type = table.Column<BoardingCardType>(type: "boarding_card_type", nullable: false),
                number = table.Column<string>(type: "text", nullable: false),
                departure = table.Column<string>(type: "text", nullable: false),
                arrival = table.Column<string>(type: "text", nullable: false),
                seat = table.Column<string>(type: "text", nullable: true),
                gate = table.Column<string>(type: "text", nullable: true),
                counter = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_boarding_card", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "boarding_card",
            schema: "public");
    }
}
