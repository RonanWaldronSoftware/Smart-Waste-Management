using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.OldRepository.Contracts
{
    public interface IOldUnitRepository
    {
        public Task<List<Model.OldCore.Unit>> GetOldUnits();
        public Task<List<Model.OldCore.UnitReading>> GetOldUnitReadings(DateTime lastTwelveMonths, string serialNumber);
    }
}
