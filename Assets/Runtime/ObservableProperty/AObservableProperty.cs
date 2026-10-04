using System;

namespace Servable.Runtime.ObservableProperty
{
    public abstract class AObservableProperty
    {
        public abstract void AddListener(Delegate handler);
        public abstract void RemoveListener(Delegate handler);
    }
}
