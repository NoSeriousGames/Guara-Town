using System.Collections.Generic;
using UnityEngine;

namespace MigalhaSystem.Pool
{
	[CreateAssetMenu(fileName = "NewPoolCollection", menuName = "Machick/Pool/Pool Collection")]
	public class PoolCollection : ScriptableObject
	{
		public List<Pool> m_pools;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (Pool pool in m_pools)
            {
                pool.OnValidate();
            }
        }
#endif
    }
}