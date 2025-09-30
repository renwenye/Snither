using System.Collections;
using System.Collections.Generic;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    private const float spawnSeconds = 2f;
    [SerializeField] private GameObject prefab;
    private int maxPrefabCount;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
        maxPrefabCount = NetworkObjectPool.Singleton.GetMaxPoolCount(prefab);
    }

    private void SpawnFoodStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnFoodStart;
        for (int i = 0; i < 10; i++)
        {
            SpawnFood();
        }
        StartCoroutine(SpawnFoodWithTime(spawnSeconds));
    }

    private void SpawnFood()
    {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab,
            GetRandomFoodPos(), Quaternion.identity);
        obj.GetComponent<Food>().prefab = prefab;
        if (obj.IsSpawned == false) 
            obj.Spawn(true);

    }

    private IEnumerator SpawnFoodWithTime(float _seconds)
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            //Debug.Log("max: " + maxPrefabCount + " in scene :" + NetworkObjectPool.Singleton.GetInSceneCount(prefab));
            if (maxPrefabCount > NetworkObjectPool.Singleton.GetInSceneCount(prefab))
            {
                SpawnFood();
                yield return new WaitForSeconds(_seconds);

            }
            else
                break;
        }
    }

    private Vector3 GetRandomFoodPos()
    {
        return new Vector3(Random.Range(-12f, 12f), Random.Range(-6f, 6f), 0);
    }
}
