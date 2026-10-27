using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWaste.Model.OldCore;

namespace SmartWaste.OldRepository.Contracts
{
    public interface IOldUserRepository
    {
        public Task<List<User>> GetOldUsers();
    }
}
