using SmartWaste.Model.OldCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.OldRepository.Contracts
{
    public interface IOldAlertRepository
    {
        public Task<List<AlertDefn>> GetOldAlerts();
    }
}
