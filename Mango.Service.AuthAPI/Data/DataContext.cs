using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Mango.Service.AuthAPI.Data
{
	public class DataContext : IdentityDbContext<IdentityUser>
    {
		public DataContext(DbContextOptions<DataContext> options):base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
	}
}

