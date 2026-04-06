namespace CalismaTakip.Services;

public interface ITakipGecmisiService
{
    /// <summary>Verilen tarih aralığındaki günlük kayıtları yeni→eski sıralı döndürür.</summary>
    Task<TakipGecmisiQueryResult> GetHistoryAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken = default);
}
