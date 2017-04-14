using Microsoft.EntityFrameworkCore;
using ffxivList.Models;

namespace ffxivList.Data
{
    public class FFListContext : DbContext
    {
        public FFListContext(DbContextOptions<FFListContext> options) : base(options)
        {
        }

        public DbSet<Levemete> Levemetes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Levemete>().ToTable("Levemete");
        }
    }
}
