using System.Collections;
using UnityEngine;
using OWS.ObjectPooling;

/// <summary>
/// this class was used to generate the demo at the start of the video
/// </summary>
public class Spawner : MonoBehaviour
{
    public GameObject spherePrefab;

    private static ObjectPool<PoolObject> spherePool;
    public bool canSpawn = true;

    private void OnEnable()
    {
        spherePool = new ObjectPool<PoolObject>(spherePrefab);

        StartCoroutine(SpawnOverTime());
    }

    IEnumerator SpawnOverTime()
    {
        while (canSpawn)
        {
            Spawn();
            yield return null;
        }
    }

    public void Spawn()
    {
        GameObject prefab;

        prefab = spherePool.PullGameObject(this.transform.position, this.transform.rotation);
    }
}