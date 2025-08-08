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

namespace StationAdvertService.Services
{
    internal sealed class HttpClientService : IHttpClientService
    {
        public HttpClient _httpClient;

        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClient  = httpClientFactory.CreateClient();
            SetupHttpClient();
        }


        private void SetupHttpClient()
        {

            _httpClient.BaseAddress = new Uri("http://connect.newag.pl:18081");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<string> GetAdvertsAsync(string url)
        {
            var response = await _httpClient.GetAsync("/ads/playlists/19/1");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task DownloadFileAsync(string url, string destinationPath)
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(destinationPath);
            await stream.CopyToAsync(fileStream);
        }
    }
}
