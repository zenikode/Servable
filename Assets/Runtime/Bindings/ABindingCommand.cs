using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableReference;
using UnityEngine;

namespace Servable.Runtime.Bindings
{

    public abstract class ABindingCommand<TPayload> : ABinding
    {
        [SerializeField]
        private ObservableCommandReference<TPayload> reference;
        
        protected ObservableCommand<TPayload> Command => reference.Observable;

        protected virtual void Awake() => Command?.AddListener(OnCommand);

        protected virtual void OnDestroy() => Command?.RemoveListener(OnCommand);

        public virtual void OnCommand(TPayload payload) { }
        
        public override bool IsValid() => reference.IsValid();
        public override Object GetModel() => reference.viewModel;
    }

    
    public abstract class ABindingCommand : ABinding
    {
        [SerializeField]
        private ObservableCommandReference reference;
        
        protected ObservableCommand Command => reference.Observable;

        protected virtual void Awake() => Command?.AddListener(OnCommand);

        protected virtual void OnDestroy() => Command?.RemoveListener(OnCommand);

        public virtual void OnCommand() { }


        public override bool IsValid() => reference.IsValid();
        public override Object GetModel() => reference.viewModel;
    }
}