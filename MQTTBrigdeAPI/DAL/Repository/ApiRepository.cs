using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StationAPI.Abstract.DAL;
using StationAPI.DAL.Context;
using StationAPI.Models;
using System.Data;
using System.Reflection.PortableExecutable;

namespace StationAPI.DAL.Repository
{
    internal class ApiRepository : IApiRepository
    {
        private readonly ILogger<ApiRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        //private readonly DbContext _context;
        public ApiRepository(ILogger<ApiRepository> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task<List<string>> GetAdvertPlaylist(int stationId,int platform)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ApiDbContext dbContext;
                try
                {
                    dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd połączenia SQL : " + ex.Message);
                    return JsonConvert.DeserializeObject<List<string>>("[]")!;
                }
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
                        return JsonConvert.DeserializeObject<List<string>>("[]")!;
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = _configuration["DatabaseConfig:GetAdvertPlaylist"];
                        command.Parameters.Add(new SqlParameter { ParameterName = "@StationId", Value = stationId });
                        command.Parameters.Add(new SqlParameter { ParameterName = "@Platform", Value = platform });

                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    if (reader.IsDBNull(0))
                                    {
                                        return JsonConvert.DeserializeObject<List<string>>("[]")!;
                                    }
                                    return
                                    JsonConvert.DeserializeObject<List<string>>(reader.GetString(0))!;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Błąd wykonania procedury SQL : " + ex.Message);
                            return JsonConvert.DeserializeObject<List<string>>("[]")!;
                        }
                    }

                    return JsonConvert.DeserializeObject<List<string>>("[]")!;
                }
            }
        }



            public async Task<List<Train>> GetGtfsKMPositions()
            {
            using (var scope = _serviceProvider.CreateScope())
            {
                ApiDbContext dbContext;
                try
                {
                    dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Błąd połączenia SQL : " + ex.Message);
                    return JsonConvert.DeserializeObject<List<Train>>("[]");
                }
                using (var context = dbContext)
                {

                    /*var connection = context.Database.GetDbConnection();
                    try
                    {
                        connection.Open();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("SQL Connection Error: " + ex.Message);
                        return JsonConvert.DeserializeObject<List<Train>>("[]");
                    }*/

                    try
                    {
                        var trains = await context.ActiveKmTrains.ToListAsync();
                        return trains;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }

                    /*using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GetCurrentCourses";
                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    if (reader.IsDBNull(0))
                                    {
                                        return JsonConvert.DeserializeObject<List<Train>>("[]");
                                    }
                                    return
                                    JsonConvert.DeserializeObject<List<Train>>(reader.GetString(0));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Błąd połączenia SQL : " + ex.Message);
                            return JsonConvert.DeserializeObject<List<Train>>("[]");
                        }
                    }*/

                    return JsonConvert.DeserializeObject<List<Train>>("[]");
                }


            }

            }

    }
}
