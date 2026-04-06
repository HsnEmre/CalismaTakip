namespace CalismaTakip.Data;

public interface IDatabaseInitializer
{
    /// <summary>Veritabanını oluşturur (yoksa) ve boşsa örnek veriyi yükler.</summary>
    void Initialize();
}
