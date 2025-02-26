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
    public class ApiRepository : IApiRepository
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

        public List<string> GetAdvertPlaylist(int stationId)
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
                    return JsonConvert.DeserializeObject<List<string>>("[]");
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
                        return JsonConvert.DeserializeObject<List<string>>("[]");
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = _configuration["DatabaseConfig:GetAdvertPlaylist"];
                        command.Parameters.Add(new SqlParameter { ParameterName = "@StationId", Value = stationId });
                        try
                        {
                            using (var reader = command.ExecuteReader())
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
                            _logger.LogError("Błąd połączenia SQL : " + ex.Message);
                            return JsonConvert.DeserializeObject<List<string>>("[]");
                        }
                    }

                    return JsonConvert.DeserializeObject<List<string>>("[]");
                }
            }
        }
    }
}
