using Microsoft.EntityFrameworkCore;
using WebApplicationFiles.Models;

namespace WebApplicationFiles.Services
{
    public class AppDbPubContext : DbContext
    {

        public AppDbPubContext(DbContextOptions options) :base(options)
        {

        }

        public DbSet<Producto> Productos { get; set; }

    }
}
