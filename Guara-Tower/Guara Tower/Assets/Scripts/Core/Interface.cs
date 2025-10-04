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

    [System.Serializable]
    public struct LevelStatus {

        public float m_BaseValue;
        public float[] m_AditionalByLevel;

        public float GetCurrentValue(int _Level) {
            return m_BaseValue + (_Level == 0 ? 0 : m_AditionalByLevel[_Level - 1]);
        }

    }

    public enum ArrowUpgrades { Damage = 0, FireRate = 1, ProjectileAmount = 2, KnockBack = 3, Ricochet = 4}

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

    public interface IProjectile {

        public void Initialize(DamageData _DamageData, float _KnockBackForce, int _RicochetAmount);

    }

}