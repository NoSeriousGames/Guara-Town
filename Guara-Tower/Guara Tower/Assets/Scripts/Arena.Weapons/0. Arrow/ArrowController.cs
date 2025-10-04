using GuaraTower.Arena.Enemies;
using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using GuaraTower.Data.Save;
using MigalhaSystem.Pool;
using UnityEngine;

namespace GuaraTower.Arena.Weapons {

    public class ArrowController : MonoBehaviour, IWeaponController {

        private bool m_Initialized = false;

        public WeaponScriptable _WeaponData;

        public LevelStatus m_DamageLevel;
        public LevelStatus m_FireRateLevel;
        public LevelStatus m_ProjectileAmountLevel;
        public LevelStatus m_KnockBackLevel;
        public LevelStatus m_RicochetLevel;

        public PoolData m_ProjectilePoolData;

        float m_CurrentTime;

        public void Initialize() {

            m_Initialized = true;
            m_CurrentTime = 1f/m_FireRateLevel.GetCurrentValue(WeaponSaveUtil.GetUpgradeLevel(_WeaponData.m_ID, (int)ArrowUpgrades.FireRate));

        }

        private void Update() {

            if (!m_Initialized) return;

            m_CurrentTime -= Time.deltaTime;
            if (m_CurrentTime > 0) return;

            m_CurrentTime = 1f/m_FireRateLevel.GetCurrentValue(WeaponSaveUtil.GetUpgradeLevel(_WeaponData.m_ID, (int)ArrowUpgrades.FireRate));

            var enemyNearest = EnemiesHelper.GetNearest(transform.position, EnemiesHelper.AimFilter);
            var dir = Vector3.Scale(enemyNearest.GetTransform().position - transform.position, new Vector3(1, 0, 1)).normalized;

            int projectileAmount = (int)m_ProjectileAmountLevel.GetCurrentValue(WeaponSaveUtil.GetUpgradeLevel(_WeaponData.m_ID, (int)ArrowUpgrades.ProjectileAmount));
            int side = 0;
            int angleMultiplier = 0;

            for (int i = 0; i < projectileAmount; i++) {

                side = (i % 2f > 0) ? 1 : -1;
                if (side == -1) angleMultiplier++;

                SpawnProjectile(dir, 5 * side * angleMultiplier);

            }

        }

        private void SpawnProjectile(Vector3 _Dir, float _ExtraAngle) {

            var projectile = m_ProjectilePoolData.PullObject(transform.position, Quaternion.identity);

            projectile.transform.forward = _Dir;
            projectile.transform.eulerAngles += new Vector3(0, _ExtraAngle, 0);

            projectile.GetComponent<IProjectile>().Initialize(
                new DamageData(m_DamageLevel.GetCurrentValue(WeaponSaveUtil.GetUpgradeLevel(_WeaponData.m_ID, (int)ArrowUpgrades.Damage)), DamageMode.Projectile, _Source: DamageSource.Player),
                m_KnockBackLevel.GetCurrentValue(WeaponSaveUtil.GetUpgradeLevel(_WeaponData.m_ID, (int)ArrowUpgrades.KnockBack)),
                (int)m_RicochetLevel.GetCurrentValue(WeaponSaveUtil.GetUpgradeLevel(_WeaponData.m_ID, (int)ArrowUpgrades.Ricochet)));

        }

    }

}