using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RaycastService : IRaycastHandler
{
    private Transform debugTransform;
    RaycastHit hit = new RaycastHit();
    private SettingsRaycaster settings;
    
    [Inject]
    public RaycastService(Transform targetPoint, SettingsRaycaster settings)
    {
        debugTransform = targetPoint;

        this.settings = settings;
    }

    public void UpdateMe() => Raycaster();

    public GameObject GetHitGameObject() => hit.collider != null && CheckLayers() ? hit.collider.gameObject : null;

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
