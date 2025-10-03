using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena {

    public abstract class AbstractProjectile : MonoBehaviour, IProjectile {

        protected DamageData m_DamageData;
        protected float m_KnockBackForce;
        protected float m_RicochetAmount;

        public float m_MoveSpeed;

        public virtual void Initialize(DamageData _DamageData, float _KnockBackForce, int _RicochetAmount) {

            m_DamageData = _DamageData;
            m_KnockBackForce = _KnockBackForce;
            m_RicochetAmount = (int)_RicochetAmount;

        }

        private void Update() {

            transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;

        }

    }

}