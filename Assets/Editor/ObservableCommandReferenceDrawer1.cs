using System;
using System.Linq;
using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableReference;
using UnityEditor;

namespace Servable.Editor
{
    [CustomPropertyDrawer(typeof(ObservableCommandReference<>))]
    public class ObservableCommandReferenceDrawer1: ObservableReferenceDrawer
    {
    
        public override bool CheckPropertyType(Type type, Type propertyGenArg)
        {
            if (!type.IsGenericType) return false;
            if (!type.GetGenericTypeDefinition().IsEquivalentTo(typeof(ObservableCommand<>))) return false;
            var genArg = type.GenericTypeArguments.FirstOrDefault();
            if (genArg != propertyGenArg) return false;
            return true;
        }
    }
}