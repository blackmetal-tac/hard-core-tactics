using UnityEngine;
using OWS.ObjectPooling;

/// <summary>
/// this class was used to generate the demo at the start of the video
/// </summary>
public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    private static ObjectPool<PoolObject> objectsPool;

    private void OnEnable()
    {
        objectsPool = new ObjectPool<PoolObject>(objectToSpawn);        
    }

    public void Spawn()
    {
        GameObject prefab;
        prefab = objectsPool.PullGameObject(this.transform.position, this.transform.rotation);
    }
}