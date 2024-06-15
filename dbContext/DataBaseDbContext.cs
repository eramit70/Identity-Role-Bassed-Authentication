using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoleBassedAuthentication.Models;

namespace RoleBassedAuthentication.dbContext
{
    public class DataBaseDbContext : IdentityDbContext<ApplicationUser>
    {
        public DataBaseDbContext(DbContextOptions<DataBaseDbContext> options) : base(options)
        {

        }
    }
}
