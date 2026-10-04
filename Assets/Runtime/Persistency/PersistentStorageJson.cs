using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Servable.Runtime.Persistency
{
    [CreateAssetMenu(menuName = "Servable/Persistency/PersistentStorageJson")]
    public class PersistentStorageJson : PersistentStorageAbstract
    {
        [SerializeField] private string filePathRelative = "prefs";

        private readonly object _sync = new();
        private Dictionary<string, JToken> _cache;
        private string FilePath => Path.IsPathRooted(filePathRelative) ? 
            filePathRelative : Path.Combine(Application.persistentDataPath, filePathRelative) + ".json";

        private void EnsureLoaded()
        {
            if (_cache != null) return;
            lock (_sync)
            {
                if (_cache != null) return;
                if (File.Exists(FilePath))
                {
                    var text = File.ReadAllText(FilePath);
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _cache = new Dictionary<string, JToken>();
                        return;
                    }

                    try
                    {
                        var jo = JObject.Parse(text);
                        _cache = jo.Properties().ToDictionary(p => p.Name, p => p.Value);
                    }
                    catch
                    {
                        _cache = new Dictionary<string, JToken>();
                    }
                }
                else
                {
                    _cache = new Dictionary<string, JToken>();
                }
            }
        }

        public bool HasKey(string key)
        {
            EnsureLoaded();
            lock (_sync) return _cache.ContainsKey(key);
        }

        public override T Get<T>(string key, T defaultValue = default)
        {
            EnsureLoaded();
            lock (_sync)
            {
                if (_cache.TryGetValue(key, out var token))
                {
                    try
                    {
                        return token.ToObject<T>();
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }

                return defaultValue;
            }
        }

        public override void Set<T>(string key, T value)
        {
            EnsureLoaded();
            lock (_sync)
            {
                _cache[key] = value == null ? JValue.CreateNull() : JToken.FromObject(value);
                SaveInternal();
            }
        }

        public override void AddListener(Action reconnect)
        {
            // Unneeded
        }

        public void RemoveKey(string key)
        {
            EnsureLoaded();
            lock (_sync)
            {
                if (_cache.Remove(key)) SaveInternal();
            }
        }

        private void SaveInternal()
        {
            var jo = new JObject();
            foreach (var kv in _cache) jo[kv.Key] = kv.Value;
            try
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(FilePath, jo.ToString(Formatting.Indented));
            }
            catch
            {
                // intentionally swallow IO errors
            }
        }
    }
}