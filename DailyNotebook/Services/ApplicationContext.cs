using DailyNotebook.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyNotebook.Services
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; } = null!;
        public DbSet<Subtask> Subtasks { get; set; } = null!;
        public DbSet<Worksheet> Worksheets { get; set; } = null!;

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=DailyNotebookDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>().Ignore(x => x.DateRange);
            modelBuilder.Entity<Subtask>().Ignore(x => x.DateRange);
            modelBuilder.Entity<Worksheet>().Ignore(x => x.LastOpenedString);
            modelBuilder.Entity<Worksheet>().Ignore(x => x.TasksCount);
        }
    }
}
