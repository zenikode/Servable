using System;
using Servable.Runtime.Extension;
using Servable.Runtime.ObservableProperty;

namespace Servable.Runtime.ObservableReference
{
    [Serializable]
    public class ObservableDataReference<T>: AObservableReference<ObservableData<T>>
    {
        public override ObservableData<T> GetProperty() => viewModel.GetData<T>(property);
    }
}