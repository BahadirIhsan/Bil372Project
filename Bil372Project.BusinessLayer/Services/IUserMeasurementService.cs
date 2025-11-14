using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public interface IUserMeasurementService
{
    // Kullanıcının ölçü bilgisi varsa getir, yoksa null
    Task<UserMeasureInput?> GetMeasureAsync(int userId);

    // Kullanıcının ölçü bilgilerini ekle/güncelle
    Task SaveMeasureAsync(int userId, UserMeasureInput input);
    
    // bu kısımda kullanıcıya özel dashboard istatistiklerini getirir
    Task<DashboardStatsDto> GetDashboardStatsAsync(int userId);

}