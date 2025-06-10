using ep_synoptic_2005.Data;
using ep_synoptic_2005.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.UploadFiles
                .Where(f => f.UploadedByUserId == userId)
                .ToListAsync();
        }

        public async Task<UploadFile> GetByIdAsync(int id)
        {
            return await _context.UploadFiles
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
