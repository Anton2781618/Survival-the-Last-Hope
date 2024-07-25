using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GlobalMapBuilding : MonoBehaviour
{
    [SerializeField] private LevelLoading levelLoading;
    public int ScenId;

    [SerializeField] private GameObject text;
    [SerializeField] private OutlineSystem.Outline _outline;


    public void ShowText(bool isShow) => text.gameObject.SetActive(isShow);

    private void OnMouseUp() 
    {
        levelLoading.Load(ScenId);
        
        Debug.Log("OnSelect");
        
    }

    void OnMouseEnter()
    {
        _outline.enabled = true;

        ShowText(true);

        text.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
    }

    void OnMouseExit()
    {
        ShowText(false);

        _outline.enabled = false;
    }
}
