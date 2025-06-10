using ep_synoptic_2005.Data;
using ep_synoptic_2005.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// <summary>
// This class implements the IUploadFileRepository interface for managing file uploads in the application.
// It provides methods to save files, retrieve files by user, and get a file by its ID.
// It uses Entity Framework Core to interact with the database.
// </summary>
namespace ep_synoptic_2005.Services
{
    public class UploadFileRepository : IUploadFileRepository
    {
        private readonly ApplicationDbContext _context;

        public UploadFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(UploadFile file)
        {
            _context.UploadFiles.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UploadFile>> GetFilesByUserAsync(string userId)
        {
            return await _context.UploadFiles.Where(f => f.UploadedByUserId == userId).ToListAsync();
        }

        public async Task<UploadFile> GetByIdAsync(int id)
        {
            return await _context.UploadFiles.FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
