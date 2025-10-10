using System;

namespace Servable.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class OnDestroyAttribute : Attribute { }
}


