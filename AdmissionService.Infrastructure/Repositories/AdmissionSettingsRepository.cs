using AdmissionService.Domain.IRepositories;
using AdmissionService.Infrastructure.Persistence;
using Contract.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AdmissionService.Infrastructure.Repositories;

public class AdmissionSettingsRepository : IAdmissionSettingRepository
{
    private readonly AdmissionDbContext _context;
    public AdmissionSettingsRepository(AdmissionDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsAdmissionOpenAsync()
    {
        var openAdmission = await _context.AdmissionSettings
            .AnyAsync(aset => aset.UniversityAdmissionStatus == UniversityAdmissionStatus.Open);
    
        return openAdmission;
    }

    public int GetAdmissionSeasonIdAsync()
    {
        var seasonId = _context.AdmissionSettings
            .First().AdmissionSeasonId;
        
        return seasonId;
    }
}