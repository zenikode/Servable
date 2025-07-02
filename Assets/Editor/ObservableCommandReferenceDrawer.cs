using System;
using System.Linq;
using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableReference;
using UnityEditor;

namespace Servable.Editor
{
    [CustomPropertyDrawer(typeof(ObservableCommandReference))]
    public class ObservableCommandReferenceDrawer: ObservableReferenceDrawer
    {
        public override bool CheckPropertyType(Type type, Type propertyGenArg)
        {
            if (!type.IsEquivalentTo(typeof(ObservableCommand))) return false;
            var genArg = type.GenericTypeArguments.FirstOrDefault();
            if (genArg != propertyGenArg) return false;
            return true;
        }
    }
}