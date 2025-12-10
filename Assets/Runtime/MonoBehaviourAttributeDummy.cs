using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using UnityEngine;

namespace Servable.Runtime
{

    public abstract class MonoBehaviourAttributeDummy : MonoBehaviourCached
    {
        protected abstract void Awake();
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void OnDestroy();
    }
}