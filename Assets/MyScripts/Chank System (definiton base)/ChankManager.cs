using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChankManager : MonoBehaviour
{
    [SerializeField] private Transform _world;
    [SerializeField] private Transform _player;
    
    public Dictionary<Vector2Int, Chank> Chanks = new Dictionary<Vector2Int, Chank>();
    private List<Vector2Int> _enableChanks = new List<Vector2Int>();

    private void Start() 
    {
        Init();

        UpdateChanks(true);
    }
    
    [ContextMenu("Init")]
    public void Init()
    {
        foreach (var item in _world.GetComponentsInChildren<Chank>(true))
        {
            item.Init();

            Chanks.Add(item.ChankPosition, item);            
        }
    }

    private void Update() => UpdateChanks();
    
    private void UpdateChanks(bool force = false)
    {
        if (force ? Time.frameCount % 1 == 0 : Time.frameCount % 100 == 0)
        {
            Vector2Int currentplayerChank = new Vector2Int(Mathf.FloorToInt(_player.position.x / 50), Mathf.FloorToInt(_player.position.z / 50));
            Vector2Int XY = new Vector2Int(Mathf.RoundToInt(_player.position.x / 50), Mathf.RoundToInt(_player.position.z / 50));
            Vector2Int XminusYminus = new Vector2Int(Mathf.RoundToInt(_player.position.x / 50) - 1, Mathf.RoundToInt(_player.position.z / 50) - 1);
            Vector2Int XminusY = new Vector2Int(Mathf.RoundToInt(_player.position.x / 50) - 1, Mathf.RoundToInt(_player.position.z / 50));
            Vector2Int XYminus = new Vector2Int(Mathf.RoundToInt(_player.position.x / 50), Mathf.RoundToInt(_player.position.z / 50) - 1);

            for (int i = 0; i < _enableChanks.Count; i++)
            {
                if (_enableChanks[i] != currentplayerChank &&
                    _enableChanks[i] != XY &&
                    _enableChanks[i] != XminusYminus &&
                    _enableChanks[i] != XminusY &&
                    _enableChanks[i] != XYminus)
                {
                    Chanks[_enableChanks[i]].gameObject.SetActive(false);

                    _enableChanks.RemoveAt(i);
                }
            }

            ActivateChank(currentplayerChank);
            ActivateChank(XY);
            ActivateChank(XminusYminus);
            ActivateChank(XminusY);
            ActivateChank(XYminus);
        }
    }

    private void ActivateChank(Vector2Int chankPosition)
    {
        if (Chanks.ContainsKey(chankPosition) && Chanks[chankPosition].gameObject.activeSelf == false)
        {
            Chanks[chankPosition].gameObject.SetActive(true);

            _enableChanks.Add(chankPosition);
        }

    }



    //включить чанки в радиусе видимости
    private void EnableChanksInViewRadius(Vector2Int currentplayerChank)
    {
        int viewRadius = 1;

        for (int x = currentplayerChank.x - viewRadius; x <= currentplayerChank.x + viewRadius; x++)
        {
            for (int z = currentplayerChank.y - viewRadius; z <= currentplayerChank.y + viewRadius; z++)
            {
                if (Chanks.ContainsKey(new Vector2Int(x, z)))
                {
                    Chanks[new Vector2Int(x, z)].gameObject.SetActive(true);
                }

            }

        }
    }
}
