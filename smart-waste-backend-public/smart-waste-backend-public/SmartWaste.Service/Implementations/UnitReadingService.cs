using AutoMapper;
using SmartWaste.Model.Core;
using SmartWaste.Repository;
using SmartWaste.Service.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Service.Implementations;

public class UnitReadingService : IUnitReadingService
{
    private readonly IDbContextFactory<RepositoryContext> _contextFactory;
    private readonly IMapper _mapper;

    public UnitReadingService(IDbContextFactory<RepositoryContext> contextFactory, IMapper mapper)
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
    }

    IRepositoryManager RepositoryManager { get => new RepositoryManager(_contextFactory.CreateDbContext()); }

    public async Task AddLatestUnitReading(LatestUnitReading latestUnitReading)
    {
        var repositoryManager = RepositoryManager;

        // Get latest unit reading
        var newLatestUnitReading = await repositoryManager.UnitReading.GetLatestUnitReadingFromLatestUnitReading(latestUnitReading.UnitId ?? 0);
        
        // Check if latest unit reading exist
        if (newLatestUnitReading != null)
        {
            newLatestUnitReading = _mapper.Map(latestUnitReading, newLatestUnitReading);
            await repositoryManager.SaveAsync();
        }
        else
            await repositoryManager.UnitReading.AddNewLatestReading(_mapper.Map(latestUnitReading, new LatestUnitReading()));
    }
}