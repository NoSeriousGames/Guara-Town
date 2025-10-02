using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena {

    public class LifeBehaviour : MonoBehaviour, ILifeSystem {

        public float m_StartLife;
        public float m_MaxLife;
        public float m_CurrentLife;

        public void Initialize(float _LifeMultiplier = 1) {

            m_MaxLife = m_StartLife * _LifeMultiplier;
            m_CurrentLife = m_MaxLife;

        }

        public float GetCurrentLife() => m_CurrentLife;

        public float GetMaxLife() => m_MaxLife;

        public Transform GetTransform() => transform;

        public bool TakeDamage(DamageData _DamageData) {
            return TakeDamage(_DamageData, out _);
        }

        public bool TakeDamage(DamageData _DamageData, out DamageData _DamageTaken) {

            _DamageTaken = _DamageData;

            if (IsAlive()) return false;

            m_CurrentLife -= _DamageData.m_Damage;

            return true;

        }

        public bool IsAlive() => m_CurrentLife > 0;

    }

}
