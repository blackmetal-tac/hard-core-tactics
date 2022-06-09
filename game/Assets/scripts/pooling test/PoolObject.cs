using System;
using UnityEngine;

namespace OWS.ObjectPooling
{
    public class PoolObject : MonoBehaviour, IPoolable<PoolObject>
    {
        private Action<PoolObject> returnToPool;

        void Update()
        {
            if (transform.localScale == Vector3.zero)
            {
                ReturnToPool();
            }           
        }

        public void Initialize(Action<PoolObject> returnAction)
        {
            //cache reference to return action
            this.returnToPool = returnAction;
        }

        public void ReturnToPool()
        {
            //invoke and return this object to pool
            returnToPool?.Invoke(this);
        }
    }
}