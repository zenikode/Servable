using System.Reflection;
using Servable.Runtime.ObservableProperty;
using UnityEngine;

namespace Servable.Runtime.Extension
{
    public static class ObservablePropertyLocatorExt
    {
        public static ObservableCommand GetCommand(this Object self, string key)
        {
            var info = self.GetType().GetProperty(key, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (info == null)
            {
                Debug.LogWarning($"ObservableCommand {key} in {self.GetType().Name} not found (Property required)",self);
                return null;
            }
            return info.GetValue(self) as ObservableCommand;
        }
        public static ObservableCommand<TPayload> GetCommand<TPayload>(this Object self, string key)
        {
            var info = self.GetType().GetProperty(key, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (info == null)
            {
                Debug.LogWarning($"ObservableCommand<{typeof(TPayload).Name}> {key} in {self.GetType().Name} not found (Property required)",self);
                return null;
            }
             
            return info.GetValue(self) as ObservableCommand<TPayload>;
        }

        public static ObservableData<TData> GetData<TData>(this Object self, string key)
        {
            var info = self.GetType().GetProperty(key, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (info == null)
            {
                Debug.LogWarning($"ObservableData<{typeof(TData).Name}> {key} in {self.GetType().Name} not found (Property required)",self);
                return null;
            }
            
            return info.GetValue(self) as ObservableData<TData>;
        }
    }
}