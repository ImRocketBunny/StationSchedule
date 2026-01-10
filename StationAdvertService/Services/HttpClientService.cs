using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Net;
using StationAdvertService.Abstract;
using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace StationAdvertService.Services
{
    internal sealed class HttpClientService : IHttpClientService
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HttpClientService> _logger;

        public HttpClientService(IHttpClientFactory httpClientFactory,ILogger<HttpClientService> logger, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;
            SetupHttpClient();
        }


        private void SetupHttpClient()
        {

            _httpClient.BaseAddress = new Uri(_configuration["HttpClientConfiguration:BaseAddress"]!);
            _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.GetValue<int>("HttpClientConfiguration:TimeOut"));
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
               $"skrmser:ZM9KwAuXVfwSB3U5QPsBeg")));
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<string> GetAdvertsAsync()
        {

            var response = await _httpClient.GetAsync("/ads/playlists/19/1");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task DownloadFileAsync(string url, string destinationPath)
        {
            try
            {
                _logger.LogInformation($"Downloading file: {url}");
                using var response = await _httpClient.GetAsync("/ads/files/{fileName}".Replace("{fileName}", url), HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = File.Create(destinationPath+ Path.AltDirectorySeparatorChar + url);
                await stream.CopyToAsync(fileStream);
                _logger.LogInformation($"Downloaded file: {url}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
           
        }
    }
}
