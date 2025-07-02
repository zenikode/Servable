using Newtonsoft.Json;
using Servable.Runtime.ObservableProperty;
using UnityEngine;

namespace Servable.Runtime.Extension
{
    public static class ConnectPrefExt
    {
        public static void ConnectPref<T>(this ObservableData<T> self, string name, T def = default)
        {
            object initialValue = def;
            switch (def)
            {
                case bool b:
                    initialValue = PlayerPrefs.GetInt(name, b ? 1 : 0) > 0;
                    break;
                
                case int i:
                    initialValue = PlayerPrefs.GetInt(name, i);
                    break;

                case float f:
                    initialValue = PlayerPrefs.GetFloat(name, f);
                    break;
                    
                case string s:
                    initialValue = PlayerPrefs.GetString(name,s);
                    break;
                    
                default:
                    if (PlayerPrefs.HasKey(name))
                    {
                        var json = PlayerPrefs.GetString(name);
                        initialValue = JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        initialValue = def;
                    }
                    break;
            }
            self.Value = (T)initialValue;

            self.AddListener(Listener);

            void Listener(T newValue)
            {
                switch (newValue)
                {
                    case bool asBool:
                        PlayerPrefs.SetInt(name, asBool ? 1 : 0);
                        break;

                    case int asInt:
                        PlayerPrefs.SetInt(name, asInt);
                        break;

                    case float asFloat:
                        PlayerPrefs.SetFloat(name, asFloat);
                        break;

                    case string asString:
                        PlayerPrefs.SetString(name, asString);
                        break;

                    default:
                        PlayerPrefs.SetString(name, JsonConvert.SerializeObject(newValue));
                        break;
                }
            }
        }
    }
}