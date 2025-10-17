using Servable.Runtime.Attributes;
using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableReference;
using UnityEngine;

namespace Servable.Runtime
{
    public abstract class ABindingData<TData> : ABinding
    {
        [SerializeField] private ObservableDataReference<TData> reference;
        public override bool IsValid() => reference.IsValid();
        public override Object GetModel() => reference.viewModel;
        protected ObservableData<TData> Data => reference.Observable;
        [Observe(nameof(Data))] public virtual void OnValue(TData value) { }
    }
}