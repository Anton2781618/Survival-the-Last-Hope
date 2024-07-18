using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
