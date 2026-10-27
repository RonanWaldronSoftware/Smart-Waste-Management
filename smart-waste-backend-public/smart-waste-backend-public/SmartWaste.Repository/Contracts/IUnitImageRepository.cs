using SmartWaste.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Repository.Contracts
{
    public interface IUnitImageRepository
    {
        /// <summary>
        /// Create unit image
        /// </summary>
        /// <param name="newUnitImage"></param>
        /// <returns></returns>
        public Task CreateUnitImage(UnitImage newUnitImage);

        /// <summary>
        /// Update unit image
        /// </summary>
        /// <param name="newUnitImage"></param>
        /// <returns></returns>
        public Task UpdateUnitImage(UnitImage newUnitImage);

        /// <summary>
        /// Get unit image by unit Id
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public Task<UnitImage?> GetUnitInstallationImage(int unitId);
    }
}
