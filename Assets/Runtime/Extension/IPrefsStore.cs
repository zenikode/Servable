namespace Servable.Runtime.Extension
{
    // Общий интерфейс для хранилищ конфигураций (PlayerPrefs wrapper, file-based JsonPrefs и т.д.)
    public interface IPrefsStore
    {
        T Get<T>(string key, T defaultValue = default);
        void Set<T>(string key, T value);
    }
}

