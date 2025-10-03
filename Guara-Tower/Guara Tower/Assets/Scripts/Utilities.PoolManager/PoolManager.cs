using Game.Utilities.Singleton;
using MigalhaSystem.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace MigalhaSystem.Pool {

    public class PoolManager : Singleton<PoolManager> {

        [SerializeField, HideInInspector] bool m_removeEmptyPool = true;
        [SerializeField] List<Pool> m_pools = new List<Pool>();
        [SerializeField] List<PoolCollection> m_poolCollections = new List<PoolCollection>();

        public bool m_RemoveEmptyPool { get { return m_removeEmptyPool; } set { m_removeEmptyPool = value; } }
        public List<Pool> m_Pools => m_pools;
        public List<PoolCollection> m_PoolCollections => m_poolCollections;

        protected override void Awake() {

            base.Awake();
            SetupPoolManager();

        }

        void SetupPoolManager() {

            AddPoolCollections();
            List<Pool> startedPools = new();
            List<Pool> duplicatePool = new();

            foreach (Pool pool in m_pools) {

                if (pool.m_PoolData == null) {

                    Debug.LogError("Empty pool found!", this);
                    continue;

                }

                if (startedPools.Exists(x => x.m_PoolData == pool.m_PoolData)) {

                    Debug.LogError("Duplicate pool found!", this);
                    duplicatePool.Add(pool);
                    continue;

                }

                startedPools.Add(pool);
                SetPool(pool);

            }

            m_pools.RemoveAll(duplicatePool);
            if (m_removeEmptyPool) m_pools.RemoveAll(x => x.m_PoolData == null);

        }

        void SetPool(Pool pool) {

            string NormalizeParentName() {
                if (pool.m_PoolData.name.ToUpper().EndsWith("POOL")) return pool.m_PoolData.name.Capitalize();
                return $"{pool.m_PoolData.name} Pool".Capitalize();
            }

            GameObject poolParent;

            if (pool.m_PoolData.m_PoolFather == null) {
                poolParent = new GameObject(NormalizeParentName());
            } else {
                poolParent = Instantiate(pool.m_PoolData.m_PoolFather);
                poolParent.name = NormalizeParentName();
            }

            poolParent.transform.ResetTransformation();            

            pool.SetupPool(poolParent.transform);
            for (int i = 0; i < pool.m_PoolData.m_PoolSize; i++) {
                pool.CreateObject();
            }

        }

        public bool ContainsPool(PoolData _poolData) => m_pools.Exists(pool => pool.ComparePoolData(_poolData));

        public Pool GetPool(PoolData _poolData) { GetPool(_poolData, out Pool _Pool); return _Pool; }
        public bool GetPool(PoolData _poolData, out Pool _Pool) {

            _Pool = null;

            List<Pool> pools = m_pools.FindAll(x => x.ComparePoolData(_poolData));
            if (!PoolCheck(pools)) return false;
            _Pool = pools[0];
            return true;

            bool PoolCheck(List<Pool> availablePools) {
                if (availablePools == null) {
                    return false;
                }
                if (availablePools.Count <= 0) {
                    return false;
                }
                if (availablePools.Count > 1) {
                    return false;
                }
                return true;
            }

        }

        public bool AddPool(PoolData _poolData) {

            if(GetPool(_poolData, out _)) return false;
            Pool newPool = new(_poolData);
            m_pools.Add(newPool);
            SetPool(newPool);
            return true;

        }

        public bool DeletePool(PoolData _poolData) {

            if (!GetPool(_poolData, out Pool pool)) return false;
            pool.DeletePool();
            m_pools.Remove(pool);
            return true;

        }

        public GameObject PullObject(PoolData _poolData, bool _createPoolNotFound = true) {

            return PullObject(_poolData, null, _createPoolNotFound);

        }

        public GameObject PullObject(PoolData _poolData, System.Action<GameObject> action, bool _createPoolNotFound = true) {

            if (GetPool(_poolData, out Pool pool)) return pool.PullObject(action);
            if (!_createPoolNotFound) return null;
            Pool newPool = new(_poolData);
            m_pools.Add(newPool);
            SetPool(newPool);
            return newPool.PullObject(action);

        }

        public GameObject PullObject(PoolData _poolData, Vector3 _position, Quaternion _rotation, bool _createPoolNotFound = true)
            => PullObject(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; }, _createPoolNotFound);

        public GameObject PullObject(PoolData _poolData, Vector3 _position, Quaternion _rotation, System.Action<GameObject> action, bool _createPoolNotFound = true)
            => PullObject(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; action?.Invoke(x); }, _createPoolNotFound);

        public T PullObject<T>(PoolData _poolData, bool _createPoolNotFound = true) {

            if (GetPool(_poolData, out Pool pool)) return pool.PullObject<T>();
            if (_createPoolNotFound == false) return default(T);
            return CreatePool(_poolData).PullObject<T>();

        }

        public T PullObject<T>(PoolData _poolData, System.Action<GameObject> action, bool _createPoolNotFound = true) {

            if (GetPool(_poolData, out Pool pool)) return pool.PullObject<T>(action);
            if (_createPoolNotFound == false) return default(T);
            return CreatePool(_poolData).PullObject<T>(action);

        }

        public Pool CreatePool(PoolData _poolData) {

            if (GetPool(_poolData, out Pool pool)) { return pool; }
            pool = new(_poolData);
            m_pools.Add(pool);
            SetPool(pool);
            return pool;

        }

        public T PullObject<T>(PoolData _poolData, Vector3 _position, Quaternion _rotation, bool _createPoolNotFound = true)
            => PullObject<T>(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; }, _createPoolNotFound);
        public T PullObject<T>(PoolData _poolData, Vector3 _position, Quaternion _rotation, System.Action<GameObject> action, bool _createPoolNotFound = true)
            => PullObject<T>(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; action?.Invoke(x); }, _createPoolNotFound);
        public bool PullObject<T>(PoolData _poolData, out T _component, bool _createPoolNotFound = true) {
            _component = PullObject<T>(_poolData, _createPoolNotFound);
            return _component != null;
        }
        public bool PullObject<T>(PoolData _poolData, out T _component, System.Action<GameObject> action, bool _createPoolNotFound = true) {
            _component = PullObject<T>(_poolData, action, _createPoolNotFound);
            return _component != null;
        }
        public bool PullObject<T>(PoolData _poolData, out T _component, Vector3 _position, Quaternion _rotation, bool _createPoolNotFound = true) {
            _component = PullObject<T>(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; }, _createPoolNotFound);
            return _component != null;
        }
        public bool PullObject<T>(PoolData _poolData, out T _component, Vector3 _position, Quaternion _rotation, System.Action<GameObject> action, bool _createPoolNotFound = true) {
            _component = PullObject<T>(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; action?.Invoke(x); }, _createPoolNotFound);
            return _component != null;
        }
        public List<GameObject> PullAllObjects(PoolData _poolData, bool _createPoolNotFound = true) {
            return PullAllObjects(_poolData, null, _createPoolNotFound);
        }
        public List<GameObject> PullAllObjects(PoolData _poolData, System.Action<GameObject> action, bool _createPoolNotFound = true) {

            if (GetPool(_poolData, out Pool pool)) return pool.PullAllObjects(action);
            if (_createPoolNotFound == false) return null;
            Pool newPool = new(_poolData);
            m_pools.Add(newPool);
            SetPool(newPool);
            return newPool.PullAllObjects(action);

        }
        public List<GameObject> PullAllObjects(PoolData _poolData, Vector3 _position, Quaternion _rotation, bool _createPoolNotFound = true)
            => PullAllObjects(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; }, _createPoolNotFound);

        public List<GameObject> PullAllObjects(PoolData _poolData, Vector3 _position, Quaternion _rotation, System.Action<GameObject> action, bool _createPoolNotFound = true)
           => PullAllObjects(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; action?.Invoke(x); }, _createPoolNotFound);

        public List<T> PullAllObjects<T>(PoolData _poolData, bool _createPoolNotFound = true) where T : Component {
            return PullAllObjects<T>(_poolData, null, _createPoolNotFound);
        }

        public List<T> PullAllObjects<T>(PoolData _poolData, System.Action<GameObject> action, bool _createPoolNotFound = true) where T : Component {
            
            if (GetPool(_poolData, out Pool pool)) return pool.PullAllObjects<T>(action);
            if (_createPoolNotFound == false) return null;
            Pool newPool = new(_poolData);
            m_pools.Add(newPool);
            SetPool(newPool);
            return newPool.PullAllObjects<T>(action);

        }

        public List<T> PullAllObjects<T>(PoolData _poolData, Vector3 _position, Quaternion _rotation, bool _createPoolNotFound = true) where T : Component
            => PullAllObjects<T>(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; }, _createPoolNotFound);

        public List<T> PullAllObjects<T>(PoolData _poolData, Vector3 _position, Quaternion _rotation, System.Action<GameObject> action, bool _createPoolNotFound = true) where T : Component
            => PullAllObjects<T>(_poolData, x => { x.transform.position = _position; x.transform.rotation = _rotation; action?.Invoke(x); }, _createPoolNotFound);

        public bool PushObject(PoolData _poolData, GameObject _gameObject, bool _createPoolNotFound = false) {
            return PushObject(_poolData, _gameObject, null, _createPoolNotFound);
        }

        public bool PushObject(PoolData _poolData, GameObject _gameObject, System.Action<GameObject> action, bool _createPoolNotFound = false) {
            
            if (GetPool(_poolData, out Pool pool)) {
                return pool.PushObject(_gameObject, action);
            }
            if (_createPoolNotFound == false) return false;
            Pool newPool = new(_poolData);
            m_pools.Add(newPool);
            SetPool(newPool);
            return newPool.PushObject(_gameObject, action);

        }

        public bool PushAllObjects(PoolData _poolData, bool _createPoolNotFound = false) {
            return PushAllObjects(_poolData, null, _createPoolNotFound);
        }

        public bool PushAllObjects(PoolData _poolData, System.Action<GameObject> action, bool _createPoolNotFound = false) {
            
            if (GetPool(_poolData, out Pool pool)) {

                return pool.PushAllObjects(action);

            }

            if (_createPoolNotFound == false) return false;
            Pool newPool = new(_poolData);
            m_pools.Add(newPool);
            SetPool(newPool);
            return newPool.PushAllObjects(action);

        }

        public void AddPoolCollections() {

            for (int i = m_poolCollections.Count - 1; i >= 0; i--) {

                PoolCollection collection = m_poolCollections[i];
                m_poolCollections.RemoveAt(i);
                if (collection == null) continue;
                if (collection.m_pools == null || collection.m_pools.Count <= 0) continue;

                foreach (Pool pool in collection.m_pools) {

                    if (pool == null || pool.m_PoolData == null || ContainsPool(pool.m_PoolData)) continue;
                    m_pools.Add(pool);

                }

            }

        }

        private void Update() {

            for (int i = m_pools.Count - 1; i >= 0; i--) {

                Pool pool = m_pools[i];
                PoolData poolData = pool.m_PoolData;
                if (!poolData.m_RemoveIdlePool) continue;
                pool.UpdateIdleTime(Time.deltaTime);

                if (pool.m_IdleTime >= poolData.m_IdlePoolLifeTime) {

                    pool.DeletePool();
                    m_pools.RemoveAt(i);

                }

            }

        }

#if UNITY_EDITOR

        private void OnValidate() {

            foreach (Pool pool in m_pools) {

                pool.OnValidate();

            }

        }

#endif
    }

    [System.Serializable]
    public class Pool {
        [field: SerializeField, HideInInspector] public string m_PoolTag { get; private set; }
        #region Variables
        [SerializeField] PoolData m_poolData;

        Transform m_poolParent;
        public Transform PoolParent { get { return m_poolParent; } private set { m_poolParent = value; } }
        List<GameObject> m_freeObjects;
        List<GameObject> m_inUseObjects;
        float m_idleTime = 0f;
        #region Getters
        public Transform m_PoolParent => m_poolParent;
        public PoolData m_PoolData => m_poolData;
        public List<GameObject> m_FreeObjects => m_freeObjects;
        public List<GameObject> m_InUseObjects => m_inUseObjects;
        public int m_ObjectAmount => m_inUseObjects.Count + m_freeObjects.Count;
        public float m_IdleTime => m_idleTime;
        #endregion
        #endregion
        #region Methods
#if UNITY_EDITOR
        public void OnValidate() {
            if (m_poolData == null) {
                m_PoolTag = "No pool!";
            } else {
                m_PoolTag = m_poolData.name;
            }

        }
#endif
        public Pool(PoolData poolData) {
            m_poolData = poolData;
            m_freeObjects = new List<GameObject>();
            m_inUseObjects = new List<GameObject>();
            m_poolParent = null;
        }
        public bool ComparePoolData(PoolData _poolData) => m_poolData == _poolData;
        public void SetupPool(Transform _parent) {
            m_freeObjects = new List<GameObject>();
            m_inUseObjects = new List<GameObject>();
            m_poolParent = _parent;
            m_idleTime = 0;
        }
        public void UpdateIdleTime(float deltaTime) {
            m_idleTime += deltaTime;
        }
        public void ClearNull() {

            Profiler.BeginSample("Clear Null");

            m_freeObjects?.RemoveAll((x)=>x == null);
            m_inUseObjects?.RemoveAll((x) => x == null);

            Profiler.EndSample();

        }
        public void DeletePool() {
            if (m_poolParent != null && m_ObjectAmount > 0) {
                m_poolParent.DestroyChildren();
                Object.Destroy(m_poolParent.gameObject);
            }
            m_freeObjects?.Clear();
            m_inUseObjects?.Clear();
        }
        public void CreateObject() {
            GameObject newGameObject = Object.Instantiate(m_poolData.m_Prefab, m_poolParent);
            m_freeObjects.Add(newGameObject);
            newGameObject.SetActive(false);
        }
        public GameObject PullObject(System.Action<GameObject> action = null) {
            m_idleTime = 0;
            if (AllowCreateNewObject()) {
                CreateObject();
            }
            if (!FreeGameObjects()) {
                return null;
            }

            GameObject poolGameObject = m_FreeObjects[m_FreeObjects.Count - 1];
            m_freeObjects.Remove(poolGameObject);
            m_inUseObjects.Add(poolGameObject);

            if (action != null) {
                action?.Invoke(poolGameObject);
            }

            poolGameObject.SetActive(true);

            IPullable[] pullableArray = poolGameObject.GetComponentsInChildren<IPullable>();
            if (PullableAvailable()) {
                foreach (IPullable pullableItem in pullableArray) {
                    pullableItem.OnPull();
                }
            }

            bool FreeGameObjects() {
                if (m_freeObjects == null) return false;
                if (m_freeObjects.Count <= 0) return false;
                return true;
            }
            bool AllowCreateNewObject() {
                if (m_poolParent == null) return false;
                if (m_freeObjects.Count > 0) return false;
                if (m_ObjectAmount >= m_poolData.MaxPoolSize())
                {
                    return m_poolData.m_ExpandablePool;
                }
                return true;
            }
            bool PullableAvailable() {
                if (pullableArray == null) return false;
                if (pullableArray.Length <= 0) return false;
                return true;
            }
            return poolGameObject;
        }
        public T PullObject<T>(System.Action<GameObject> action = null) {
            var pulledObject = PullObject(action);
            if (pulledObject != null && pulledObject.TryGetComponent(out T component)) return component;
            Debug.LogError($"{typeof(T)} component not found!");
            return default(T);
        }
        public List<GameObject> PullAllObjects(System.Action<GameObject> action = null) {
            m_idleTime = 0;
            List<GameObject> gameObjects = new();
            while (m_freeObjects.Count >= 1) {
                gameObjects.Add(PullObject(action));
            }

            return gameObjects;
        }
        public List<T> PullAllObjects<T>(System.Action<GameObject> action = null) {
            ClearNull();
            m_idleTime = 0;
            List<T> gameObjects = new();
            while (m_freeObjects.Count >= 1) {
                gameObjects.Add(PullObject<T>(action));
            }
            return gameObjects;
        }
        public bool PushObject(GameObject _activeGameObject, System.Action<GameObject> action = null) {

            if (!m_inUseObjects.Contains(_activeGameObject) && !m_freeObjects.Contains(_activeGameObject)) {
#if DEBUG
                Debug.LogError(_activeGameObject.name + " Não Pertence a Pool" + m_PoolData.name);
#endif
                return false;
            }

            if (m_freeObjects.Contains(_activeGameObject)) {
#if DEBUG
                Debug.LogWarning(_activeGameObject.name + " Já Está Na Pool " + m_PoolData.name);
#endif
                return true;
            }

                m_idleTime = 0;
            IPushable[] pushableArray = _activeGameObject.GetComponentsInChildren<IPushable>(true);
            if (IPushableAvailable()) {
                foreach (IPushable pushableItem in pushableArray) {
                    pushableItem.OnPush();
                }
            }
            m_freeObjects.Add(_activeGameObject);
            m_inUseObjects.Remove(_activeGameObject);
            if (action != null) {
                action?.Invoke(_activeGameObject);
            }

            _activeGameObject.transform.SetParent(m_poolParent);
            _activeGameObject.SetActive(false);

            if (!m_poolData.m_DestroyExtraObjectsAfterUse) return true;

            if (m_ObjectAmount <= m_poolData.MaxPoolSize()) return true;
            m_freeObjects.Remove(_activeGameObject);
            Object.Destroy(_activeGameObject);
            return true;
            bool IPushableAvailable() {
                if (pushableArray == null) return false;
                if (pushableArray.Length <= 0) return false;
                return true;
            }
        }
        public bool PushAllObjects(System.Action<GameObject> action = null) {
            ClearNull();
            m_idleTime = 0;
            bool result = true;
            while (m_inUseObjects.Count >= 1) {
                if (!PushObject(m_inUseObjects[0], action)) result = false;
            }
            return result;
        }
        #endregion
    }

    public interface IReturnToPool {
        public void ReturnToPool();
    }
    public interface IPoolable : IPullable, IPushable, IReturnToPool {
    }
    public interface IPullable { void OnPull(); }
    public interface IPushable { void OnPush(); }

}