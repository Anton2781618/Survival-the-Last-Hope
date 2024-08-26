using UnityEngine;
using UnityEngine.AI;

namespace MyProject
{
    public interface IMuveHandler
    {
        public void SetTarget(Transform target);

        public bool TargetIsNull();
        public bool IsMoveNow();
        public void UpdateMe();
    }
}
