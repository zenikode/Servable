using Object = UnityEngine.Object;

namespace Servable.Runtime
{
    public abstract class ABinding : MonoBehaviourExtended
    {
        public abstract bool IsValid();
        public abstract Object GetModel();
    }
}