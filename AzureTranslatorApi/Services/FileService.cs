using Azure.Storage;
using Azure.Storage.Blobs;
using AzureTranslatorApi.DTO;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;

namespace AzureTranslatorApi.Services
{
    public class FileService
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

        public async Task<BlobResponseDto> UploadAsync(IBrowserFile blob)
        {
            BlobResponseDto response = new();
            BlobClient client = _filesContainerInputs.GetBlobClient(blob.Name);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.Name} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

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
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;
                    var content = await file.DownloadContentAsync();

                    string name = blobFiles.FirstOrDefault().Name;
                    string contentType = content.Value.Details.ContentType;

                    return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
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
                    BlobClient file = _filesContainerInputs.GetBlobClient(_file.Name);

                    await file.DeleteAsync();

                    response.Add(new BlobResponseDto { Error = false, Status = $"File: {_file} has been successfully deleted." });
                }
            }
            return response;
        }

    }
}
