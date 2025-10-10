using System;

namespace Servable.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class OnDataAttribute : Attribute
    {
        public readonly string PropertyName;
        public OnDataAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}


