using System;
using Mango.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace Mango.Services.EmailAPI.Data
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

        public DbSet<EmailLogger> EmailLoggers { get; set; }
	}
}

