using Microsoft.EntityFrameworkCore.Migrations;

// all migrations can be applied for example this one with the command  dotnet ef database update
namespace DatingApp.API.Migrations
{
    //What is a partial class ?
    public partial class InitialCase : Migration
    {
        // This is where the migration actually does something
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Takes the name of the table from the dbcontext. One is the Id column and one is the Name column and does different table 
                // related operations while creating , setting soem constraints etc. We can alter this if we want otherwise.
            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });
        }

        // This is the opposite of the Up method where thee table is just dropped
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
