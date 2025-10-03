using UnityEngine;

namespace Game.Utilities.Singleton
{

    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {

        [SerializeField, Tooltip("Don't Destroy On Load")] protected bool m_DDOL;
        private static T instance;
        public static T Instance => GetInstance(false);
        public static bool IsNull => instance == null;

        protected virtual void Awake()
        {

            if (Instance != null && Instance != this)
            {

                Destroy(gameObject);
                Debug.Log("Singleton Destroy "+gameObject.name);
                return;

            }

            if (m_DDOL)
            {

                DontDestroyOnLoad(gameObject);

            }

        }

        public static T GetInstance(bool createIfNullInstance = true)
        {

            if (!IsNull) return instance;

            instance = FindFirstObjectByType<T>();

            if (!IsNull || !createIfNullInstance) return instance;

            if (Application.isPlaying)
            {

                GameObject singletonObject = new GameObject(typeof(T).Name);
                instance = singletonObject.AddComponent<T>();
                instance.SetInstanceSettings();

            }
            else
            {

                Debug.LogWarning($"{typeof(T).Name} Singleton is missing and is not created in edit mode.");

            }

            return instance;

        }

        public virtual void SetInstanceSettings() {}

    }

}