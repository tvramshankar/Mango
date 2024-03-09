﻿using Mango.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace Mango.Services.ShoppingCartAPI.Data
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

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
    }
}

