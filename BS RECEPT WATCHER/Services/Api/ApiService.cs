using BS_RECEPT_WATCHER.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5252/");
        }
        public async Task<string> CreateUnitStorage(UnitStorage unitStorage)
        {
            var uri = "api/UnitStorage/CreateUnitStorage";

            var jsonUserData = JsonConvert.SerializeObject(unitStorage);
            var content = new StringContent(jsonUserData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse;
                }
                else
                {
                    return "Erreur : " + response.StatusCode;
                }
            }
            catch (Exception e)
            {
                return "Erreur lors de la demande : " + e.Message;
            }
        }

        public async Task<string> GetLatestUidWithUnitStorage(string unitStorage)
        {
            var uri = $"api/UnitStorage/GetLatestUidForUnitstorage?uid={unitStorage}";

            var response = await _httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<string>(content);
        }
    }
}
