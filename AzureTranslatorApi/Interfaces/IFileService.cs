using AzureTranslatorApi.DTO;

namespace AzureTranslatorApi.Interfaces
{
    public interface IFileService
    {
        Task<List<BlobResponseDto>> DeleteInputsAsync();
        Task<List<BlobResponseDto>> DeleteOutputsAsync();
        Task<BlobDto> DownloadAsync();
        Task<ICollection<BlobDto>> ListInputsAsync();
        Task<ICollection<BlobDto>> ListOutputsAsync();
        Task<BlobResponseDto> UploadAsync(string filePath);
    }
}
