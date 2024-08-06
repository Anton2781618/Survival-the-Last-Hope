using System.Collections.Generic;
using Units;
using UnityEngine;

namespace MyProject
{
    public class GameDataBase : MonoBehaviour
    {
        private Dictionary<Collider, Unit> units = new Dictionary<Collider, Unit>();

        public void AddUnit(Collider collider, Unit unit)
        {
            units.Add(collider, unit);        
        }

        public void RemoveUnit(Collider collider)
        {
            units.Remove(collider);
        }

        public Unit GetUnit(Collider collider)
        {
            return units[collider];
        }

        [ContextMenu("Показать юниты")]
        public void ShowUnits()
        {
            foreach (var unit in units)
            {
                Debug.Log(unit.Value.name);
            }
        }
    }

}
