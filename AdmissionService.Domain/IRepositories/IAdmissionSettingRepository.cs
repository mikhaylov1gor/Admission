namespace AdmissionService.Domain.IRepositories;

public interface IAdmissionSettingRepository
{
    Task<bool> IsAdmissionOpenAsync();
    int GetAdmissionSeasonIdAsync();
}