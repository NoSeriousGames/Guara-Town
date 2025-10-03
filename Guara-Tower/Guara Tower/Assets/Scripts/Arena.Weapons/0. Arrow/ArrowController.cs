using GuaraTower.Arena.Enemies;
using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using MigalhaSystem.Pool;
using UnityEngine;

namespace GuaraTower.Arena.Weapons {

    public class ArrowController : MonoBehaviour {

        private bool m_Initialized = false;

        public PoolData m_ProjectilePoolData;

        public LevelStatus m_DamageLevel;
        public LevelStatus m_FireRateLevel;
        public LevelStatus m_ProjectileAmountLevel;
        public LevelStatus m_KnockBackLevel;
        public LevelStatus m_RicochetLevel;

        public int m_CurrentLevel;
        float m_CurrentTime;

        public void Initialize() {

            m_Initialized = true;
            m_CurrentTime = m_FireRateLevel.GetCurrentValue(m_CurrentLevel);

        }

        private void Update() {

            if (!m_Initialized) return;

            m_CurrentTime -= Time.deltaTime;
            if (m_CurrentTime > 0) return;

            m_CurrentTime = m_FireRateLevel.GetCurrentValue(m_CurrentLevel);

            var enemyNearest = EnemiesHelper.GetNearest(transform.position, EnemiesHelper.AimFilter);
            var dir = Vector3.Scale(enemyNearest.GetTransform().position - transform.position, new Vector3(1, 0, 1)).normalized;

            int projectileAmount = (int)m_ProjectileAmountLevel.GetCurrentValue(m_CurrentLevel);
            int side = 0;
            int angleMultiplier = 0;

            for (int i = 0; i < projectileAmount; i++) {

                side = (i % 2f > 0) ? 1 : -1;
                if (side == -1) angleMultiplier++;

                SpawnProjectile(dir, 5 * side * angleMultiplier);

            }

        }

        private void SpawnProjectile(Vector3 _Dir, float _ExtraAngle) {

            var projectile = m_ProjectilePoolData.PullObject();
            projectile.transform.right = _Dir;
            projectile.transform.eulerAngles += new Vector3(0, _ExtraAngle, 0);
            projectile.GetComponent<IProjectile>().Initialize(
                new DamageData(m_DamageLevel.GetCurrentValue(m_CurrentLevel), DamageMode.Projectile, _Source: DamageSource.Player),
                m_KnockBackLevel.GetCurrentValue(m_CurrentLevel),
                (int)m_RicochetLevel.GetCurrentValue(m_CurrentLevel));

        }

    }

}