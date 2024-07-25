using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoading : MonoBehaviour
{
    [SerializeField] LoadMenuUI loadMenuUI;
    AsyncOperation asyncOperation;

    public void Load(int ScenId)
    {
        loadMenuUI.ShowMenu(true);
        
        StartCoroutine(LoadScene(ScenId));
    }
    
    IEnumerator LoadScene(int ScenId)
    {
        yield return new WaitForSeconds(1);

        asyncOperation = SceneManager.LoadSceneAsync(ScenId);

        while (!asyncOperation.isDone)
        {
            loadMenuUI.UdateSlider(asyncOperation.progress / 0.9f);
            
            yield return null;
        }
    }
}
