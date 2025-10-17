using System;
using JetBrains.Annotations;

namespace Servable.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    [MeansImplicitUse]
    public sealed class ObserveAttribute : Attribute 
    {
        public readonly string PropertyName;
        public ObserveAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}


