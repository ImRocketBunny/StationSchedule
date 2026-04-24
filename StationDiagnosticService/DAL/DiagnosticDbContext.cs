using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.DAL
{
    internal class DiagnosticDbContext : DbContext
    {
        public DiagnosticDbContext(DbContextOptions<DiagnosticDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
