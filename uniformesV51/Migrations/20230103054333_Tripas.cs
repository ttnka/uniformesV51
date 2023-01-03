using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uniformesV51.Migrations
{
    public partial class Tripas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsuariosId",
                table: "Bitacora",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bitacora",
                newName: "UsuariosId");
        }
    }
}
