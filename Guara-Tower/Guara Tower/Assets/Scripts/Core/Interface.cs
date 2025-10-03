using UnityEngine;

namespace GuaraTower.Core.Data {

    public enum DamageElement { Normal = 0 }
    public enum DamageMode { none = 0, Melee = 1, Projectile = 2, Area = 3}
    public enum DamageSource { none = 0, Player = 1, Enemy = 2}

    [System.Serializable]
    public struct DamageData {

        public float m_Damage;
        public DamageMode m_Mode;
        public DamageElement m_Element;
        public DamageSource m_Source;

        public DamageData(float _Damage = 0, 
            DamageMode _Mode = DamageMode.none, 
            DamageElement _Element = DamageElement.Normal,
            DamageSource _Source = DamageSource.none) {

            m_Damage = _Damage;
            m_Mode = _Mode;
            m_Element = _Element;
            m_Source = _Source;

        }

    }


}

namespace GuaraTower.Core.Interface {
    using GuaraTower.Core.Data;

    public interface ILifeSystem : IAlive, ITakeDamage {

        void Initialize(float _LifeMultiplier);
        float GetMaxLife();
        float GetCurrentLife();
        Transform GetTransform();

    }

    public interface IAlive {
        bool IsAlive();
    }

    public interface ITakeDamage {
        bool TakeDamage(DamageData _DamageData);
        bool TakeDamage(DamageData _DamageData, out DamageData _DamageTaken);

    }

    public interface ITarget {

        ILifeSystem GetLifeSystem();
        Transform GetTransform();

    }

    public interface IPlayerTarget : ITarget { }

    public interface IEnemyTarget : ITarget {

        public IPlayerTarget PlayerTarget { get; }
        public Vector3 TargetDir { get; }
        public float TargetDistance { get; }

    }

    public interface IIgnoreAim { }

}