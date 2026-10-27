using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWaste.Model.Core;
using SmartWaste.Model.OldCore;
using SmartWaste.OldRepository.Contracts;
using SmartWaste.OldRepository.Implementation;
using Microsoft.Identity.Client;

namespace SmartWaste.OldRepository
{
    public class OldRepositoryManager : IOldRepositoryManager
    {
        private readonly MediatoriumSmartWasteContext _repositoryContext;

        private readonly Lazy<IOldAlertRepository> _alertRepository;
        private readonly Lazy<IOldUnitRepository> _oldUnitRepository;
        private readonly Lazy<IOldUserRepository> _oldUserRepository;

        public OldRepositoryManager(MediatoriumSmartWasteContext repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _alertRepository = new Lazy<IOldAlertRepository>(() => new OldAlertRepository(repositoryContext));
            _oldUnitRepository = new Lazy<IOldUnitRepository>(() => new OldUnitRepository(repositoryContext));
            _oldUserRepository = new Lazy<IOldUserRepository>(() => new OldUserRepository(repositoryContext));
            
        }

        public IOldAlertRepository OldAlert => _alertRepository.Value;
        public IOldUnitRepository OldUnit => _oldUnitRepository.Value;
        public IOldUserRepository OldUser => _oldUserRepository.Value;
    }
}
