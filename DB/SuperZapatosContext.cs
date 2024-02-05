using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class SuperZapatosContext : DbContext
    {

        public SuperZapatosContext(DbContextOptions<SuperZapatosContext> options) 
        : base(options)
        { 
            
        }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Stores> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articles>().ToTable("articles");
            modelBuilder.Entity<Stores>().ToTable("stores");
        }
    }
}
