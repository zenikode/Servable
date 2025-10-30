using Newtonsoft.Json;
using UnityEngine;

namespace Servable.Runtime.Extension
{
    public class PlayerPrefsStore : IPrefsStore
    {
        private static PlayerPrefsStore _instance;
        public static PlayerPrefsStore Instance => _instance ??= new PlayerPrefsStore();


        public T Get<T>(string key, T def = default)
        {
            object result = def;
            switch (def)
            {
                case bool b:
                    result = PlayerPrefs.GetInt(key, b ? 1 : 0) > 0;
                    break;
                
                case int i:
                    result = PlayerPrefs.GetInt(key, i);
                    break;

                case float f:
                    result = PlayerPrefs.GetFloat(key, f);
                    break;
                    
                case string s:
                    result = PlayerPrefs.GetString(key,s);
                    break;
                    
                default:
                    if (PlayerPrefs.HasKey(key))
                    {
                        var json = PlayerPrefs.GetString(key);
                        result = JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        result = def;
                    }
                    break;
            }

            return (T)result;
        }

        public void Set<T>(string key, T value)
        {
            switch (value)
            {
                case bool asBool:
                    PlayerPrefs.SetInt(key, asBool ? 1 : 0);
                    break;

                case int asInt:
                    PlayerPrefs.SetInt(key, asInt);
                    break;

                case float asFloat:
                    PlayerPrefs.SetFloat(key, asFloat);
                    break;

                case string asString:
                    PlayerPrefs.SetString(key, asString);
                    break;

                default:
                    PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value));
                    break;
            }
        }
    }
}
