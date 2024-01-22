using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFront.Services
{
    public class FileService
    {
        private readonly HttpClient _httpClient;

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeleteInputsAsync()
        {
            var response = await _httpClient.DeleteAsync("https://localhost:7297/Files/Inputs/");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception($"Error fetching: {response.StatusCode}");
            }
        }
        
        public async Task<bool> DeleteOutputsAsync()
        {
            var response = await _httpClient.DeleteAsync("https://localhost:7297/Files/Outputs/");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception($"Error fetching: {response.StatusCode}");
            }
        }

        public async Task<string> UploadAsync(IBrowserFile file)
        {
            StreamContent streamContent = new StreamContent(file.OpenReadStream())
            {
                Headers =
            {
                ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"file\"",
                    FileName = "\"" + file.Name + "\""
                },
                ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType)
            }
            };

            var response = await _httpClient.PostAsync("https://localhost:7297/Files/Upload/", streamContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                throw new Exception($"Error fetching: {response.StatusCode}");
            }
        }

        public async Task DownloadFile() 
        {
            var response = await _httpClient.GetAsync("https://localhost:7297/Files/Download/");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching: {response.StatusCode}");
            }
            else
            {


            }
        }

    }
}
