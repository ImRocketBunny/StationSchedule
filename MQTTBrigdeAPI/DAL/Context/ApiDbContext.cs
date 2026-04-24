using Microsoft.EntityFrameworkCore;
using StationAPI.Models;
using System;

namespace StationAPI.DAL.Context
{
    public class ApiDbContext : DbContext
    {

        //public DbSet<Train> ActiveKmTrains { get; set; } = null!;

        public DbSet<TrainDetails> GetTrainDetails { get; set; } = null!;
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            /*modelBuilder.Entity<Train>()
                .HasNoKey()
                .ToView("RunningTrains");*/

            modelBuilder.Entity<TrainDetails>()
                .HasNoKey()
                .ToView("GetTrain");
                
                
            modelBuilder.Entity<TrainDetails>()
              .Property(e => e.stop_lon)
                .HasPrecision(9, 6); ;

            modelBuilder.Entity<TrainDetails>()
               .Property(e => e.stop_lat)
                .HasPrecision(9, 6);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
