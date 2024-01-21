using AzureTranslatorApi.Interfaces;
using System.Text;

namespace AzureTranslatorApi.Services
{
    public class TranslatorService : ITranslator
    {
        private static readonly string endpoint = "https://classeswithazure.cognitiveservices.azure.com/translator/text/batch/v1.1";
        private static readonly string key = "e4dc4696a9cb4f6c80d4cdf2fb60eb70";
        static readonly string route = "/batches";

        public async Task<bool> TranslateAsync(string inLanguage, string outLanguage)
        {
            string sourceURL = "\"https://azureblobsforclasses.blob.core.windows.net/inputs?sp=rl&st=2024-01-21T14:05:19Z&se=2024-01-27T22:05:19Z&skoid=a004217c-7ae2-42f2-b475-f602c4bc4856&sktid=ab840be7-206b-432c-bd22-4c20fdc1b261&skt=2024-01-21T14:05:19Z&ske=2024-01-27T22:05:19Z&sks=b&skv=2022-11-02&spr=https&sv=2022-11-02&sr=c&sig=nChVcJWz94JY%2BHRK5Xm57bluIkpsSCwWMLULC%2BjA5tI%3D\"";
            string targetURL = " \"https://azureblobsforclasses.blob.core.windows.net/outputs?sp=wl&st=2024-01-21T14:00:53Z&se=2024-01-27T22:00:53Z&skoid=a004217c-7ae2-42f2-b475-f602c4bc4856&sktid=ab840be7-206b-432c-bd22-4c20fdc1b261&skt=2024-01-21T14:00:53Z&ske=2024-01-27T22:00:53Z&sks=b&skv=2022-11-02&spr=https&sv=2022-11-02&sr=c&sig=AA2P3RA%2F1nBS2BqKmLZY9e0lY6U9jDYG5%2FrioxmAaRo%3D\"";

            string json = "{\"inputs\": [{\"source\": {\"sourceUrl\":" + sourceURL + " ,\"storageSource\": \"AzureBlob\",\"language\": \""+inLanguage+"\"}, \"targets\": [{\"targetUrl\":" + targetURL + ",\"storageSource\": \"AzureBlob\",\"category\": \"general\",\"language\": \""+outLanguage+"\"}]}]}";

            using HttpClient client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage();
            {
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                        //$"Status code: {response.StatusCode}\n\nResponse Headers:\n{response.Headers}";
                }
                else
                {
                    return false;
                        //"Error";
                }
            }
        }
    }
}
