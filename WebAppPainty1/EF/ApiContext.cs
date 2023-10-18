using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAppPainty1
{


    public class ApiContext : IdentityDbContext
    {
        public new DbSet<User>  Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
       

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        

    }


}
