using System.Collections.Generic;
using UnityEngine;
using System;

namespace OWS.ObjectPooling
{
    public class ObjectPool<T> : IPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        public ObjectPool(GameObject pooledObject, int numToSpawn = 0)
        {
            this.prefab = pooledObject;
            Spawn(numToSpawn);
        }

        public ObjectPool(GameObject pooledObject, Action<T> pullObject, Action<T> pushObject, int numToSpawn = 0)
        {
            this.prefab = pooledObject;
            this.pullObject = pullObject;
            this.pushObject = pushObject;
            Spawn(numToSpawn);
        }

        private System.Action<T> pullObject;
        private System.Action<T> pushObject;
        private Stack<T> pooledObjects = new Stack<T>();
        private GameObject prefab;
        public int pooledCount
        {
            get
            {
                return pooledObjects.Count;
            }
        }

        public T Pull()
        {
            T t;
            if (pooledCount > 0)
                t = pooledObjects.Pop();
            else
                t = GameObject.Instantiate(prefab).GetComponent<T>();

            //ensure the object is on
            t.Initialize(Push);

            //allow default behavior and turning object back on
            pullObject?.Invoke(t);

            return t;
        }

        public T Pull(Vector3 position)
        {
            T t = Pull();
            t.transform.position = position;
            return t;
        }

        public T Pull(Vector3 position, Quaternion rotation)
        {
            T t = Pull();
            t.transform.position = position;
            t.transform.rotation = rotation;
            return t;
        }

        public GameObject PullGameObject()
        {
            return Pull().gameObject;
        }

        public GameObject PullGameObject(Vector3 position, float size, float damage)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            Explosion explosion = go.GetComponentInChildren<Explosion>();
            explosion.Damage = damage;
            explosion.Spawn(size);
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation, float size, float speed)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.gameObject.transform.localScale = size * Vector3.one; //ensure the object is on            
            go.GetComponentInChildren<Rigidbody>().velocity = speed * go.transform.forward;
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation, float size, float damage, float speed)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.gameObject.transform.localScale = size * Vector3.one; //ensure the object is on
            go.GetComponentInChildren<Projectile>().Damage = damage;
            go.GetComponentInChildren<Rigidbody>().velocity = speed * go.transform.forward;
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation, float size, float damage, float speed, string bulletID)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.gameObject.transform.localScale = size * Vector3.one; //ensure the object is on
            Projectile projectile = go.GetComponentInChildren<Projectile>();
            projectile.Damage = damage;
            projectile.BulletID = bulletID;
            go.GetComponentInChildren<Rigidbody>().velocity = speed * go.transform.forward;
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation, float size, float damage, float speed, Vector3 target, bool isFriend)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.gameObject.transform.localScale = size * Vector3.one; //ensure the object is on
            Missile missile = go.GetComponentInChildren<Missile>();
            missile.MissileCollider.enabled = true;
            if (isFriend)
            {
                missile.MissileCollider.gameObject.layer = 12;
            }
            else
            {
                missile.MissileCollider.gameObject.layer = 13;
            }
            missile.Homing = false;
            missile.Target = target;
            missile.Damage = damage;
            missile.Speed = speed;
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation, float size, float damage, float speed, GameObject target, bool isFriend)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.gameObject.transform.localScale = size * Vector3.one; //ensure the object is on
            Missile missile = go.GetComponentInChildren<Missile>();
            missile.MissileCollider.enabled = true;
            if (isFriend)
            {
                missile.MissileCollider.gameObject.layer = 12;
            }
            else
            {
                missile.MissileCollider.gameObject.layer = 13;
            }
            missile.Homing = true;
            missile.HomingTarget = target;
            missile.Damage = damage;
            missile.Speed = speed;
            return go;
        }

        public void Push(T t)
        {
            pooledObjects.Push(t);

            //create default behavior to turn off objects
            pushObject?.Invoke(t);

            t.gameObject.transform.localScale = Vector3.zero;
        }

        private void Spawn(int number)
        {
            T t;

            for (int i = 0; i < number; i++)
            {
                t = GameObject.Instantiate(prefab).GetComponent<T>();
                pooledObjects.Push(t);
                t.gameObject.transform.localScale = Vector3.zero;
            }
        }
    }

    public interface IPool<T>
    {
        T Pull();
        void Push(T t);
    }

    public interface IPoolable<T>
    {
        void Initialize(System.Action<T> returnAction);
        void ReturnToPool();
    }
}