using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiService.Python
{
    public class PythonClient
    {
        private readonly HttpClient _httpClient;

        public PythonClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // New: parse a single raw input into structured user details
        public async Task<string> ParseUserDetailsAsync(string rawInput)
        {
            var payload = new { input = rawInput };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // New Python endpoint that uses Groq internally
            var response = await _httpClient.PostAsync("/api/userdetails/parse", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // (Optional) Keep the old method if other parts still use it
        // public async Task<string> ClassifyTodo(string description) { ... }
    }
}
