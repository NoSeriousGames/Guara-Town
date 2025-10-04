using GuaraTower.Arena.Enemies;
using GuaraTower.Core.Data;
using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena {

    public class ArrowProjectile : AbstractProjectile {

        public override string m_TargetTag { get => "Enemy"; }

        protected override void TargetHitted(ITakeDamage _TakeDamage, DamageData _DamageData) {

            var enemyNearest = EnemiesHelper.GetNearest(transform.position, EnemiesHelper.AimFilter);
            var dir = Vector3.Scale(enemyNearest.GetTransform().position - transform.position, new Vector3(1, 0, 1)).normalized;

            transform.forward = dir;

            base.TargetHitted(_TakeDamage, _DamageData);

        }

    }

}