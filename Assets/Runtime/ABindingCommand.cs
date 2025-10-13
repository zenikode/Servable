using Servable.Runtime.Attributes;
using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableReference;
using UnityEngine;

namespace Servable.Runtime
{

    public abstract class ABindingCommand<TPayload> : ABinding
    {
        [SerializeField]
        private ObservableCommandReference<TPayload> reference;
        public override bool IsValid() => reference.IsValid();
        public override Object GetModel() => reference.viewModel;
        protected ObservableCommand<TPayload> Command => reference.Observable;
        [OnCommand(nameof(Command))] public virtual void OnCommand(TPayload payload) { }
    }
    
    public abstract class ABindingCommand : ABinding
    {
        [SerializeField]
        private ObservableCommandReference reference;
        public override bool IsValid() => reference.IsValid();
        public override Object GetModel() => reference.viewModel;
        protected ObservableCommand Command => reference.Observable;
        [OnCommand(nameof(Command))] public virtual void OnCommand() { }

    }
}