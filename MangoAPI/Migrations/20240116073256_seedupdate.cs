using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "coupons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount", "MinAmount" },
                values: new object[] { 3, "30OFF", 30.0, 60 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "coupons",
                keyColumn: "CouponId",
                keyValue: 3);
        }
    }
}
