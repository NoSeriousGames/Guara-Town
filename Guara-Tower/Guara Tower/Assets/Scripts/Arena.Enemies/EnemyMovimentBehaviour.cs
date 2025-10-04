using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena.Enemies {

    public class EnemyMovimentBehaviour : MonoBehaviour {

        IEnemyTarget m_EnemyTarget;
        IEnemyTarget EnemyTarget { get => m_EnemyTarget ??= GetComponent<IEnemyTarget>(); }

        public float m_DistanceLimit = 1;
        public float m_MoveSpeed;

        private void Update() {

            if (EnemyTarget.TargetDistance <= m_DistanceLimit) return;
            transform.position += EnemyTarget.TargetDir * m_MoveSpeed * Time.deltaTime;
            transform.LookAt(EnemyTarget.PlayerTarget.GetTransform());
        }

    }

}
