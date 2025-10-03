using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena.Enemies {

    public class EnemyBehaviour : MonoBehaviour, IEnemyTarget {

        private ILifeSystem m_LifeSystem;
        public ILifeSystem GetLifeSystem() {
            return m_LifeSystem ??= GetComponent<ILifeSystem>();
        }

        public Transform GetTransform() {
            return transform;
        }

        public IPlayerTarget PlayerTarget { get => PlayerHelper.GetPlayerTarget(); }

        public Vector3 TargetDir { get => m_TargetDir; }
        private Vector3 m_TargetDir = Vector3.one;
        public float TargetDistance { get => m_TargetDistance; }
        private float m_TargetDistance = 0;

        private void Update() {

            var dirNonNormalized = (PlayerTarget.GetTransform().position - transform.position);
            m_TargetDir = dirNonNormalized.normalized;
            m_TargetDistance = dirNonNormalized.sqrMagnitude;

        }

    }

}
