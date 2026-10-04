using Servable.Runtime.ObservableProperty;

namespace Servable.Runtime.Persistency
{
    public static class PersistencyExt
    {
        public static void ConnectStorage<T>(this ObservableData<T> self, PersistentStorageAbstract storage, string name, T def = default)
        {
            self.Value = storage.Get(name, def);
            self.AddListener(Listener);
            return;
            void Listener(T newValue)
            {
                storage.Set(name, newValue);
            }
        }
    }
}