using ep_synoptic_2005.Models;
using System.Threading.Tasks;

namespace ep_synoptic_2005.Services
{
    public interface IUploadFileRepository
    {
        Task SaveAsync(UploadFile file);

        Task<List<UploadFile>> GetFilesByUserAsync(string userId);

        Task<UploadFile> GetByIdAsync(int id);





    }

}
