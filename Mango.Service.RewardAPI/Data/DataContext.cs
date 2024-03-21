using Mango.Service.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace Mango.Service.RewardAPI.Data
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

        public DbSet<Reward> Rewards { get; set; }
    }
}