using System;
using JetBrains.Annotations;

namespace Servable.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    [MeansImplicitUse]
    public sealed class OnEnableAttribute : Attribute { }
}


