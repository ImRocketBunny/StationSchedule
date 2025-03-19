using Microsoft.EntityFrameworkCore;


namespace ScheduleUpdater.DAL.Context
{
    internal class UpdaterDbContext : DbContext
    {
        public UpdaterDbContext(DbContextOptions<UpdaterDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
