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

		public DbSet<Coupon> coupons { get; set; }
	}
}

