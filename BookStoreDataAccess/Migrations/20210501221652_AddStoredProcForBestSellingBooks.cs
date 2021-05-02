using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStoreDataAccess.Migrations
{
    public partial class AddStoredProcForBestSellingBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROC bestSellers
                                    AS
                                    BEGIN
                                     SELECT TOP 5 b.Title, SUM(s.Count) as books_sold from dbo.ShoppingCarts s 
                                     LEFT JOIN dbo.Books b 
                                     ON 
                                     s.BookId = b.BookId GROUP BY b.Title ORDER BY books_sold desc
                                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE bestSellers");
        }
    }
}
