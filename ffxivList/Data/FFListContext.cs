﻿using Microsoft.EntityFrameworkCore;
using ffxivList.Models;
using System.Linq;

namespace ffxivList.Data
{
    public class FFListContext : DbContext
    {
        //public FFListContext(DbContextOptions<FFListContext> options) : base(options)
        //{
        //}

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

            modelBuilder.Entity<AllUserCraft>().HasKey(c => new { c.CraftID, c.UserID });
            modelBuilder.Entity<AllUserLevemete>().HasKey(c => new { c.LevemeteID, c.UserID });
            modelBuilder.Entity<AllUserQuest>().HasKey(c => new { c.QuestID, c.UserID });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server = localhost; database = db_fflist; uid = root; pwd = My_SQLD4t4base-;");
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
