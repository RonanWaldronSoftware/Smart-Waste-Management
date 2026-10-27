using SmartWaste.Model.OldCore;
using SmartWaste.OldRepository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.OldRepository.Implementation
{
    public class OldUnitRepository : IOldUnitRepository
    {
        private readonly MediatoriumSmartWasteContext _context;

        public OldUnitRepository(MediatoriumSmartWasteContext context)
        {
            _context = context;
        }

        public async Task<List<Model.OldCore.Unit>> GetOldUnits() => await _context.Units.ToListAsync();

        public async Task<List<Model.OldCore.UnitReading>> GetOldUnitReadings(DateTime lastTwelveMonths, string serialNumber) => await _context.UnitReadings
            .Where(r => (r.ReadingDateTime == null || r.ReadingDateTime >= lastTwelveMonths) && r.SerialNo.Equals(serialNumber)).ToListAsync();
    }
}
