using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChankManager : MonoBehaviour
{
    [SerializeField] private Transform world;
    [SerializeField] private Transform Player;
    
    public Dictionary<Vector2Int, Chank> Chanks = new Dictionary<Vector2Int, Chank>();

    private void Start() 
    {
        Init();
    }
    
    [ContextMenu("Init")]
    public void Init()
    {
        foreach (var item in world.GetComponentsInChildren<Chank>())
        {
            item.Init();

            Chanks.Add(item.ChankPosition, item);            
        }
    }

    private void Update()
    {
        Vector2Int currentplayerChank = new Vector2Int(Mathf.FloorToInt(Player.position.x / 50), Mathf.FloorToInt(Player.position.z / 50));
        Vector2Int roundedPlayerChank = new Vector2Int(Mathf.RoundToInt(Player.position.x / 50), Mathf.RoundToInt(Player.position.z / 50));

        Vector2Int playerPos = new Vector2Int((int)Player.position.x / 50 , (int)Player.position.z / 50);

        foreach (var сhank in Chanks)
        {
            сhank.Value.gameObject.SetActive(false);
        }
        Chanks[currentplayerChank].gameObject.SetActive(true);        
        
        Chanks[roundedPlayerChank].gameObject.SetActive(true);

        Chanks[new Vector2Int(Mathf.RoundToInt(playerPos.x), playerPos.y / 50)].gameObject.SetActive(true);

        Chanks[new Vector2Int(playerPos.x, Mathf.RoundToInt(playerPos.y))].gameObject.SetActive(true);

        
        Chanks[new Vector2Int(Mathf.RoundToInt(playerPos.x) - 1, Mathf.RoundToInt(playerPos.y) - 1)].gameObject.SetActive(true);
        
        Chanks[new Vector2Int(Mathf.RoundToInt(playerPos.x) - 1, Mathf.RoundToInt(playerPos.y))].gameObject.SetActive(true);

        Chanks[new Vector2Int(Mathf.RoundToInt(playerPos.x), Mathf.RoundToInt(playerPos.y) - 1)].gameObject.SetActive(true);

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
