using UnityEngine;

public class ClothAdder : MonoBehaviour
{
    [SerializeField] private GameObject _clothes;
    [SerializeField] private SkinnedMeshRenderer playerSkin;
 
 
    [ContextMenu("addClothes")]
    private void addClothes()
    {
        GameObject clothObj = Instantiate(_clothes, playerSkin.transform.parent);
        SkinnedMeshRenderer[] renderers = clothObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.bones = playerSkin.bones;
            renderer.rootBone = playerSkin.rootBone;
        }
    }
}
 