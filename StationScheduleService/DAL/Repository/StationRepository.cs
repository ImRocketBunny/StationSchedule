using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StationScheduleService.DAL.Abstract;
using StationScheduleService.DAL.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.DAL.Repository
{
    internal sealed class StationRepository : IStationRepository
    {
        private readonly ILogger<StationRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public StationRepository(ILogger<StationRepository> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider; 
        }


        public async Task<List<string>> GetStationStructure(int stationId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                StationDbContext dbContext;


                dbContext = scope.ServiceProvider.GetRequiredService<StationDbContext>();


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
                        return JsonConvert.DeserializeObject<List<string>>("[]");
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GetStationTrackStructure";//_configuration["DatabaseConfig:"];
                        command.Parameters.Add(new SqlParameter { ParameterName = "@StationId", Value = stationId });
                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    if (reader.IsDBNull(0))
                                    {
                                        return JsonConvert.DeserializeObject<List<string>>("[]");
                                    }
                                    return
                                    JsonConvert.DeserializeObject<List<string>>(reader.GetString(0));
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Błąd wykonania procedury SQL : " + ex.Message);
                            return JsonConvert.DeserializeObject<List<string>>("[]");

                        }
                    }

                    return JsonConvert.DeserializeObject<List<string>>("[]");
                }
            }   
        }
    }
}
