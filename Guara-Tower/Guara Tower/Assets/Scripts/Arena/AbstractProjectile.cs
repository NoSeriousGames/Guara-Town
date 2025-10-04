using Game.Utilities.Extensions;
using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using MigalhaSystem.Pool;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GuaraTower.Arena {

    public abstract class AbstractProjectile : MonoBehaviour, IProjectile {

        public PoolData m_PoolData;
        public virtual string m_TargetTag { get => ""; }

        protected DamageData m_DamageData;
        protected float m_KnockBackForce;
        protected float m_RicochetAmount;

        public float m_MoveSpeed;

        private List<Collider> m_HitTarget = new List<Collider>();
        CancellationTokenSource m_CancellationTokenSource;

        private void OnEnable() {

            m_CancellationTokenSource?.CancellHelper();
            m_CancellationTokenSource = new CancellationTokenSource();

        }

        private void OnDisable() {

            m_CancellationTokenSource?.CancellHelper();
            m_CancellationTokenSource = null;

        }

        public virtual void Initialize(DamageData _DamageData, float _KnockBackForce, int _RicochetAmount) {

            m_DamageData = _DamageData;
            m_KnockBackForce = _KnockBackForce;
            m_RicochetAmount = (int)_RicochetAmount;

            m_HitTarget.Clear();

        }

        private void Update() {

            transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;

        }

        public void OnTriggerEnter(Collider _Collider) {

            if (!string.IsNullOrEmpty(m_TargetTag) && _Collider.CompareTag(m_TargetTag)) return;
            DamageExtension.Damage(_Collider, m_HitTarget, m_DamageData, 1, m_CancellationTokenSource.Token, TargetHitted);

        }

        protected virtual void TargetHitted(ITakeDamage _TakeDamage, DamageData _DamageData) {

            if (m_RicochetAmount == 0) 
                m_PoolData.PushObject(gameObject);
            m_RicochetAmount -= 1;

        }

    }

}