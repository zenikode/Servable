using System;

namespace Servable.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class OnCommandAttribute : Attribute
    {
        public readonly string PropertyName;
        public OnCommandAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}


