using UnityEngine;
using UnityEngine.AI;

namespace Tool
{
    public interface IMuveHandler
    {
        public void SetTarget(Transform target);

        public bool TargetIsNull();
        public bool IsMoveNow();
        public void UpdateMe();
    }
}
