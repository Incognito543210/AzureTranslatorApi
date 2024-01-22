namespace BlazorFront.Services
{
    public class TranslateService
    {
        private readonly HttpClient _httpClient;

        public TranslateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<string> TranslateAsync(string inL, string outL)
        {
            var response = await _httpClient.GetAsync("https://localhost:7297/Api/Translator/" + inL + "," + outL);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response into a string
                var wynik = await response.Content.ReadAsStringAsync();
                return wynik;
            }
            else
            {
                throw new Exception($"Error fetching: {response.StatusCode}");
            }
        }




    }
}
