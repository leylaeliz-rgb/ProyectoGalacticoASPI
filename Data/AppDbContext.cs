using ProyectoGalactico.Models;
using Microsoft.EntityFrameworkCore;


namespace ProyectoGalactico.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CardCharacter> CardsCharacter => Set<CardCharacter>();
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Event> Events => Set<Event>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



        }
    }
}
