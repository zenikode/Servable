using System;
using UnityEngine;

namespace Servable.Runtime.Persistency
{
    // Общий интерфейс для хранилищ конфигураций (PlayerPrefs wrapper, file-based JsonPrefs и т.д.)
    public abstract class PersistentStorageAbstract : ScriptableObject
    {
        public abstract T Get<T>(string key, T defaultValue = default);
        public abstract void Set<T>(string key, T value);

        public abstract void AddListener(Action reconnect);
    }
}

