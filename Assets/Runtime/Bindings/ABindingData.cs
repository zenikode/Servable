using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableReference;
using UnityEngine;

namespace Servable.Runtime.Bindings
{
    public abstract class ABindingData<TData> : ABinding
    {
        [SerializeField]
        private ObservableDataReference<TData> reference;
        
        protected ObservableData<TData> Data => reference.Observable;

        protected virtual void Awake() => Data?.AddListener(OnValue);

        protected virtual void OnDestroy() => Data?.RemoveListener(OnValue);

        public virtual void OnValue(TData value) { }

        public override bool IsValid() => reference.IsValid();
        public override Object GetModel() => reference.viewModel;
    }
}