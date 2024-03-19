using Mango.Services.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;
namespace Mango.Services.OrderAPI.Data
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

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}

