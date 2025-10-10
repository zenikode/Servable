using System;
using JetBrains.Annotations;

namespace Servable.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    [MeansImplicitUse]
    public sealed class OnCommandAttribute : Attribute
    {
        public readonly string PropertyName;
        public OnCommandAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}


