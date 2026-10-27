using SmartWaste.Model.OldCore;
using SmartWaste.OldRepository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.OldRepository.Implementation
{
    public class OldAlertRepository : IOldAlertRepository
    {
        private readonly MediatoriumSmartWasteContext _context;

        public OldAlertRepository(MediatoriumSmartWasteContext context)
        {
            _context = context;
        }

        public async Task<List<AlertDefn>> GetOldAlerts() => await _context.AlertDefns.ToListAsync();
    }
}
