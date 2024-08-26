using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Units
{
    public abstract class Unit : MonoBehaviour
    {
        public virtual void Use(Unit unit)
        {
            
        }
    }
}