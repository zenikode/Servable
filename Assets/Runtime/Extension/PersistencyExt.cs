using Servable.Runtime.ObservableProperty;

namespace Servable.Runtime.Extension
{
    public static class PersistencyExt
    {
        public static void ConnectPref<T>(this ObservableData<T> self, string name, T def = default)
        {
            var store = PlayerPrefsStore.Instance;
            ConnectStorage(self, store, name, def);
        }
          
        public static void ConnectStorage<T>(this ObservableData<T> self, IPrefsStore store, string name, T def = default)
        {
            self.Value = store.Get(name, def);
            self.AddListener(Listener);
            return;
            void Listener(T newValue)
            {
                store.Set(name, newValue);
            }
        }
    }
}