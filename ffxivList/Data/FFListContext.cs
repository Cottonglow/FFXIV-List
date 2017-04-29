using Microsoft.EntityFrameworkCore;
using ffxivList.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ffxivList.Data
{
    public class FfListContext : DbContext
    {
        public FfListContext(DbContextOptions<FfListContext> options)
            : base(options)
        {
        }

        public DbSet<Levemete> Levemetes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Quest> Quest { get; set; }
        public DbSet<Craft> Craft { get; set; }

        public DbSet<UserLevemete> UserLevemete { get; set; }
        public DbSet<UserQuest> UserQuest { get; set; }
        public DbSet<UserCraft> UserCraft { get; set; }

        public DbSet<AllUserCraft> AllUserCraft { get; set; }
        public DbSet<AllUserLevemete> AllUserLevemete { get; set; }
        public DbSet<AllUserQuest> AllUserQuest { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Levemete>().ToTable("Levemete");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Quest>().ToTable("Quest");
            modelBuilder.Entity<Craft>().ToTable("Craft");
            modelBuilder.Entity<UserLevemete>().ToTable("UserLevemete");
            modelBuilder.Entity<UserQuest>().ToTable("UserQuest");
            modelBuilder.Entity<UserCraft>().ToTable("UserCraft");
            modelBuilder.Entity<AllUserCraft>().ToTable("AllUserCraft");
            modelBuilder.Entity<AllUserLevemete>().ToTable("AllUserLevemete");
            modelBuilder.Entity<AllUserQuest>().ToTable("AllUserQuest");

            modelBuilder.Entity<AllUserCraft>().HasKey(c => new { CraftID = c.CraftId, UserID = c.UserId });
            modelBuilder.Entity<AllUserLevemete>().HasKey(c => new { LevemeteID = c.LevemeteId, UserID = c.UserId });
            modelBuilder.Entity<AllUserQuest>().HasKey(c => new { QuestID = c.QuestId, UserID = c.UserId });
        }

        public void DetachAllEntities()
        {
            foreach (var entity in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            {
                Entry(entity.Entity).State = EntityState.Detached;
            }
        }
    }
}
