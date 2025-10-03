using Cysharp.Threading.Tasks;
using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using System.Threading;
using UnityEngine;

namespace GuaraTower.Arena.Enemies {
    public class EnemyMeleeAttackBehaviour : MonoBehaviour {

        IEnemyTarget m_EnemyTarget;
        IEnemyTarget EnemyTarget { get => m_EnemyTarget ??= GetComponent<IEnemyTarget>(); }

        public DamageData m_DamageData;
        public float m_AttackRange = 1;
        public float m_AttackCoolDown = 2;
        bool m_InCoolDown = false;

        CancellationTokenSource m_CancellationTokenSource;

        private void OnDisable() {

            m_CancellationTokenSource?.CancellHelper();
            m_CancellationTokenSource = null;
            m_InCoolDown = false;

        }

        private void Update() {

            if (EnemyTarget.TargetDistance > m_AttackRange) {

                m_CancellationTokenSource?.CancellHelper();
                m_CancellationTokenSource = null;
                m_InCoolDown = false;
                return;

            }

            if (m_InCoolDown) return;

            m_CancellationTokenSource?.CancellHelper();
            m_CancellationTokenSource = new CancellationTokenSource();
            AttackCoolDown(m_CancellationTokenSource.Token);

        }

        async UniTask AttackCoolDown(CancellationToken _CancellationToken) {

            m_InCoolDown = true;
            if (await UniTask.WaitForSeconds(m_AttackCoolDown, cancellationToken: _CancellationToken).SuppressCancellationThrow()) return;
            m_InCoolDown = false;
            EnemyTarget.PlayerTarget.GetLifeSystem().TakeDamage(m_DamageData);
            
        }

    }

}
