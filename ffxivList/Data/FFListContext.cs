using Microsoft.EntityFrameworkCore;
using ffxivList.Models;
using Microsoft.Extensions.Configuration;

namespace ffxivList.Data
{
    public class FFListContext : DbContext
    {
        //public FFListContext(DbContextOptions<FFListContext> options) : base(options)
        //{
        //}

        public DbSet<Levemete> Levemetes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Levemete>().ToTable("Levemete");
            modelBuilder.Entity<User>().ToTable("User");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server = localhost; database = db_fflist; uid = root; pwd = My_SQLD4t4base-;");
        }
    }
}
