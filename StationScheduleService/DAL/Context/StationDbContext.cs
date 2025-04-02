using Microsoft.EntityFrameworkCore;

namespace StationScheduleService.DAL.Context
{
    internal class StationDbContext : DbContext
    {
        public StationDbContext(DbContextOptions<StationDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
