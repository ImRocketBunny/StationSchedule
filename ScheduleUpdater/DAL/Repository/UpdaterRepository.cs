using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ScheduleUpdater.Abstract;
using ScheduleUpdater.DAL.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.DAL.Repository
{
    class UpdaterRepository : IUpdaterRepository
    {
        private readonly ILogger<UpdaterRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;




        public UpdaterRepository(ILogger<UpdaterRepository> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task UpdateSchedule(string content)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                UpdaterDbContext dbContext;
                
                
                dbContext = scope.ServiceProvider.GetRequiredService<UpdaterDbContext>();
                
                
                using (var context = dbContext)
                {

                    var connection = context.Database.GetDbConnection();
                    try
                    {
                        connection.Open();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("SQL Connection Error: " + ex.Message);
                        //return JsonConvert.DeserializeObject<List<string>>("[]");
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.ReceiveData_Schedule";//_configuration["DatabaseConfig:"];
                        command.Parameters.Add(new SqlParameter { ParameterName = "@jsonData", Value = content });
                        try
                        {
                            await command.ExecuteNonQueryAsync();


                            _logger.LogInformation("Insert procedure executed.");


                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Błąd wykonania procedury SQL : " + ex.Message);

                        }
                    }


                }
            }
        }
    }
}
