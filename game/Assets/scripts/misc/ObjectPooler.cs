using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size = 5;
    }

    public List<Pool> pools;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.localScale = Vector3.zero;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.localScale = Vector3.one * 0.05f;
        objectToSpawn.transform.position = position;        
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.GetComponent<Collider>().enabled = true;

        /*Projectile.target = new Vector3(
            enemy.transform.position.x + Random.Range(-PlayerController.spread, PlayerController.spread),
            enemy.transform.position.y + Random.Range(-PlayerController.spread, PlayerController.spread),
            enemy.transform.position.z + Random.Range(-PlayerController.spread, PlayerController.spread)
            ).normalized;*/

        //Projectile.target = enemy.transform.position;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
