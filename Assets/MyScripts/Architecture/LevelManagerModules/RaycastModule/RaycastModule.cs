using System.Collections;
using System.Collections.Generic;
using ModularEventArchitecture;
using UnityEngine;

namespace Entitys.LevelManagerModules.RaycastModule
{
    [CompatibleUnit(typeof(LevelManager))]
    public class RaycastModule : ModuleBase
    {
        [SerializeField] private Transform debugTransform;
        [SerializeField] private SettingsRaycaster settings;
        public override void Initialize()
        {
        }

        public override void UpdateMe()
        {
            Raycaster();
        }

        RaycastHit hit = new RaycastHit();

        public Collider GetHitCollider() => hit.collider != null && CheckLayers() ? hit.collider : null;

        private bool CheckLayers()
        {
            if((settings.Layers.value & (1 << hit.collider.gameObject.layer)) == 0) return false;

            return true;

        }

        private void Raycaster()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, 50f))
            {
                debugTransform.position = hit.point;
            }
        }
    }
}
