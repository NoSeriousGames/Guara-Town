using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena.Enemies {

    public class EnemyBehaviour : MonoBehaviour, IEnemyTarget {

        private ILifeSystem m_LifeSystem;
        public ILifeSystem GetLifeSystem() {
            return m_LifeSystem ??= GetComponent<ILifeSystem>();
        }

    }

    public class EnemyMovimentBehaviour : MonoBehaviour {

        public Vector3 m_FinalDestination;
        public float m_MoveSpeed;

    }

}
