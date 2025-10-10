using System;
using JetBrains.Annotations;

namespace Servable.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    [MeansImplicitUse]
    public sealed class OnDisableAttribute : Attribute { }
}


