using Azure.Storage;
using Azure.Storage.Blobs;
using AzureTranslatorApi.DTO;
using AzureTranslatorApi.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;

namespace AzureTranslatorApi.Services
{
    public class FileService : IFileService
    {
        private readonly string _storageAccount = "azureblobsforclasses";
        private readonly string _key = "2YNkijDoR+jXhgZcaS7bzXL77DGJdr9lLhgRMa8It2S2mULs56w9Di+mtilNR81EfCbOqYjNpVpz+AStXcB51A==";
        private readonly BlobContainerClient _filesContainerInputs;
        private readonly BlobContainerClient _filesContainerOutputs;

        public FileService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net/";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainerInputs = blobServiceClient.GetBlobContainerClient("inputs");
            _filesContainerOutputs = blobServiceClient.GetBlobContainerClient("outputs");
        }

        public async Task<ICollection<BlobDto>> ListInputsAsync()
        {
            List<BlobDto> files = new List<BlobDto>();

            await foreach (var file in _filesContainerInputs.GetBlobsAsync())
            {
                string uri = _filesContainerInputs.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";
                files.Add(new BlobDto
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }
            return files;
        }

        public async Task<ICollection<BlobDto>> ListOutputsAsync()
        {
            List<BlobDto> files = new List<BlobDto>();

            await foreach (var file in _filesContainerOutputs.GetBlobsAsync())
            {
                string uri = _filesContainerOutputs.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";
                files.Add(new BlobDto
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }
            return files;
        }

        public async Task<BlobResponseDto> UploadAsync(string filePath)
        {
            BlobResponseDto response = new BlobResponseDto();

            filePath= filePath.Trim('"');
            filePath = filePath.Replace(";", "\\");
            string fileName = Path.GetFileName(filePath);


            BlobClient client = _filesContainerInputs.GetBlobClient(fileName);

            try
            {
                // Otwórz strumień danych z pliku na dysku
                await using (Stream data = File.OpenRead(filePath))
                {
                    // Wgraj plik na Blob Storage
                    await client.UploadAsync(data);
                }

                // Ustaw informacje w odpowiedzi
                response.Status = $"File {fileName} uploaded successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
            }
            catch (Exception ex)
            {
                // Obsłuż ewentualne błędy
                response.Status = $"Error uploading file: {ex.Message}";
                response.Error = true;
            }

            return response;
        }

        public async Task<BlobDto> DownloadAsync()
        {
            var blobFiles = await ListOutputsAsync();
            if (blobFiles.Count != 0)
            {
                BlobClient file = _filesContainerOutputs.GetBlobClient(blobFiles.FirstOrDefault().Name);

                if (await file.ExistsAsync())
                {
                    var downloadResponse = await file.DownloadAsync();

                    if (downloadResponse != null && downloadResponse.Value != null)
                    {
                        string name = blobFiles.FirstOrDefault().Name;
                        string contentType = downloadResponse.Value.ContentType;

                        string projectPath = AppDomain.CurrentDomain.BaseDirectory;
                        string folderName = "TranslateDocuments";
                        string folderPath = Path.Combine(projectPath, folderName);


                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Tutaj można podać ścieżkę do folderu, gdzie chcesz zapisać plik
                        string filePath = Path.Combine(folderPath, name);

                        using (var fileStream = File.OpenWrite(filePath))
                        {
                            await downloadResponse.Value.Content.CopyToAsync(fileStream);
                            fileStream.Close();
                        }

                        return new BlobDto { Uri = null, Name = name, ContentType = contentType, FilePath = filePath };
                    }
                }
            }
            return null;

        }

        public async Task<List<BlobResponseDto>> DeleteInputsAsync()
        {
            var files = await ListInputsAsync();
            List<BlobResponseDto> response = new List<BlobResponseDto>();
            if (files.Count != 0)
            {
                foreach (var _file in files)
                {
                    BlobClient file = _filesContainerInputs.GetBlobClient(_file.Name);

                    await file.DeleteAsync();

                    response.Add(new BlobResponseDto { Error = false, Status = $"File: {_file} has been successfully deleted." });
                }
            }
                return response;
        }

        public async Task<List<BlobResponseDto>> DeleteOutputsAsync()
        {
            var files = await ListOutputsAsync();
            List<BlobResponseDto> response = new List<BlobResponseDto>();
            if (files.Count != 0)
            {
                foreach (var _file in files)
                {
                    BlobClient file = _filesContainerOutputs.GetBlobClient(_file.Name);

                    await file.DeleteAsync();

                    response.Add(new BlobResponseDto { Error = false, Status = $"File: {_file} has been successfully deleted." });
                }
            }
            return response;
        }

    }
}
