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
    public class OldUserRepository : IOldUserRepository
    {
        private readonly MediatoriumSmartWasteContext _context;

        public OldUserRepository(MediatoriumSmartWasteContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetOldUsers() => await _context.Users.ToListAsync();
    }
}
