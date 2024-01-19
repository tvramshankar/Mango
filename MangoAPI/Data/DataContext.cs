using System;
using Microsoft.EntityFrameworkCore;
using MangoAPI.Models;
namespace MangoAPI.Data
{
	public class DataContext : DbContext
    {
		public DataContext(DbContextOptions<DataContext> options):base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                DiscountAmount = 10,
                CouponCode = "10OFF",
                MinAmount = 20
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 2,
                DiscountAmount = 20,
                CouponCode = "20OFF",
                MinAmount = 40
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 3,
                DiscountAmount = 30,
                CouponCode = "30OFF",
                MinAmount = 60
            });
        }

        public DbSet<Coupon> coupons { get; set; }
	}
}

