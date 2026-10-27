using SmartWaste.OldRepository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.OldRepository
{
    public interface IOldRepositoryManager
    {
        public IOldAlertRepository OldAlert { get; }
        public IOldUnitRepository OldUnit { get; }
        public IOldUserRepository OldUser { get; }
    }
}
