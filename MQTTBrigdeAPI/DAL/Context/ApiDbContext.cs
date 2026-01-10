using Microsoft.EntityFrameworkCore;
using StationAPI.Models;
using System;

namespace StationAPI.DAL.Context
{
    public class ApiDbContext : DbContext
    {

        public DbSet<Train> ActiveKmTrains { get; set; } = null!;
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Train>()
                .HasNoKey()
                .ToView("RunningTrains");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
