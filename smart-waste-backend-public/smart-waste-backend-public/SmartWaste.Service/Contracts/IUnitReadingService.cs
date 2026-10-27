using SmartWaste.Model.Core;

namespace SmartWaste.Service.Contracts;

public interface IUnitReadingService
{
    /// <summary>
    /// Add latest unit reading into table latest unit reading
    /// </summary>
    /// <param name="latestUnitReading"></param>
    /// <returns></returns>
    Task AddLatestUnitReading(LatestUnitReading latestUnitReading);
}