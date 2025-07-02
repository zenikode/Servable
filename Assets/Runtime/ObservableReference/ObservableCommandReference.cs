using System;
using Servable.Runtime.Extension;
using Servable.Runtime.ObservableProperty;

namespace Servable.Runtime.ObservableReference
{
    [Serializable]
    public class ObservableCommandReference<T>: AObservableReference<ObservableCommand<T>>
    {
    public override ObservableCommand<T> GetProperty() => viewModel.GetCommand<T>(property);
    }
    
    [Serializable]
    public class ObservableCommandReference: AObservableReference<ObservableCommand>
    {
        public override ObservableCommand GetProperty() => viewModel.GetCommand(property);
    }
}