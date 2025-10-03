using UnityEngine;

namespace MigalhaSystem.Pool
{

    public abstract class PoolableObject : MonoBehaviour, IPoolable
    {

        [SerializeField, Tooltip("It calls the method 'On Pull' on Awake\n'On Pull' will be called twice if the pool creates the object at runtime!")]
        bool m_onPullOnAwake = true;

        protected virtual void Awake()
        {
            if (m_onPullOnAwake) OnPull();
        }

        public virtual void OnPull()
        {

        }

        public virtual void OnPush()
        {

        }

        public virtual void ReturnToPool()
        {

        }

    }

}