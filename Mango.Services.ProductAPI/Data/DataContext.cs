using System;
using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace Mango.Services.ProductAPI.Data
{
	public class DataContext : DbContext
    {
		public DataContext(DbContextOptions<DataContext> options):base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> products { get; set; }
	}
}

