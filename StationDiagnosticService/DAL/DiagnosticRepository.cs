using StationDiagnosticService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.DAL
{
    internal class DiagnosticRepository : IDiagnosticRepository
    {
        private readonly ILogger<DiagnosticRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;


        private string _insertDataProcedure;
        private string _insertDataParam;

        public DiagnosticRepository(ILogger<DiagnosticRepository> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        private void GetDbConfig()
        {
            //_insertDataParam = _configuration[]
        }

        public async Task InsertDiagnosticData(DataFrame dataFrame)
        {
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var dbContext=scope.ServiceProvider.GetRequiredService<DiagnosticDbContext>();
                //using 
            }
            
        }
    }
}
