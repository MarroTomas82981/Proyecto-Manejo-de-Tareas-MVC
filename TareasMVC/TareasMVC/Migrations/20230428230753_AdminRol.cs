using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT Id FROM AspNetRoles WHERE Id = 'c6e759d6-72c6-4873-9253-6e2d8f3df6e9')
                                    BEGIN
	                                    INSERT AspNetRoles ([Id],[Name], [NormalizedName]) 
	                                    VALUES ('c6e759d6-72c6-4873-9253-6e2d8f3df6e9','admin','ADMIN')
                                    END ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles WHERE Id = 'c6e759d6-72c6-4873-9253-6e2d8f3df6e9'");
        }
    }
}
