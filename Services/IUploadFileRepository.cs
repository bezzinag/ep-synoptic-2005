using ep_synoptic_2005.Models;


// <summary>
// This interface defines the contract for the upload file repository service.
// It includes methods for saving files, retrieving files by user, and getting a file by its ID.
// </summary>
namespace ep_synoptic_2005.Services
{
    public interface IUploadFileRepository
    {
        Task SaveAsync(UploadFile file);
        Task<List<UploadFile>> GetFilesByUserAsync(string userId);
        Task<UploadFile> GetByIdAsync(int id);
    }
}
