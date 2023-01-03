using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using uniformesV51.Model;

namespace uniformesV51.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Z100_Org> Organizaciones { get; set; }
        public DbSet<Z110_Usuarios> Usuarios { get; set; }
        public DbSet<Z190_Bitacora> Bitacora { get; set; }

    }
}