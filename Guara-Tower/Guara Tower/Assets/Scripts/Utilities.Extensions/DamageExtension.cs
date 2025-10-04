using System.Threading;
using UnityEngine;

namespace Game.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GuaraTower.Core.Data;
    using GuaraTower.Core.Interface;
    using UnityEngine.Profiling;

    public static class DamageExtension
    {

        public static void Damage(RaycastHit[] _Enemies, List<Collider> _HitTarget, DamageData _Damage, float _TimeToEnemyStayInList, CancellationToken _CancellationToken, Action<ITakeDamage, DamageData> _CallBack = null, ITakeDamage _LifeSystemExepction = null)
            => Damage(_Enemies, _HitTarget, _Damage, Vector2.zero, 0, 0, _TimeToEnemyStayInList, _CancellationToken, _CallBack, _LifeSystemExepction);
        public static void Damage(RaycastHit[] _Enemies, List<Collider> _HitTarget, DamageData _Damage, Vector2 _KnockBackPosi, float _KnockForce, float _KnockDuration, float _TimeToEnemyStayInList, CancellationToken _CancellationToken, Action<ITakeDamage, DamageData> _CallBack = null, ITakeDamage _LifeSystemExepction = null)
        {

            foreach (var collided in _Enemies)
            {

                Damage(collided.collider, _HitTarget, _Damage, _KnockBackPosi, _KnockForce, _KnockDuration, _TimeToEnemyStayInList, _CancellationToken, _CallBack, _LifeSystemExepction);

            }

        }

        public static void Damage(Collider[] collided, List<Collider> _HitTarget, DamageData _Damage, float _TimeToEnemyStayInList, CancellationToken _CancellationToken, Action<ITakeDamage, DamageData> _CallBack = null, ITakeDamage _LifeSystemExepction = null, bool _ShowDamageText = true)
            => Damage(collided, _HitTarget, _Damage, Vector2.zero, 0, 0, _TimeToEnemyStayInList, _CancellationToken, _CallBack, _LifeSystemExepction, _ShowDamageText);
        public static void Damage(Collider[] _Enemies, List<Collider> _HitTarget, DamageData _Damage, Vector2 _KnockBackPosi, float _KnockForce, float _KnockDuration, float _TimeToEnemyStayInList, CancellationToken _CancellationToken, Action<ITakeDamage, DamageData> _CallBack = null, ITakeDamage _LifeSystemExepction = null, bool _ShowDamageText = true)
        {

            foreach (var collided in _Enemies)
            {

                Damage(collided, _HitTarget, _Damage, _KnockBackPosi, _KnockForce, _KnockDuration, _TimeToEnemyStayInList, _CancellationToken, _CallBack, _LifeSystemExepction, _ShowDamageText);

            }

        }

        public static void Damage(Collider collided, List<Collider> _HitTarget, DamageData _Damage, float _TimeToEnemyStayInList, CancellationToken _CancellationToken, Action<ITakeDamage, DamageData> _CallBack = null, ITakeDamage _LifeSystemExepction = null, bool _ShowDamageText = true)
            => Damage(collided, _HitTarget, _Damage, Vector2.zero, 0, 0, _TimeToEnemyStayInList, _CancellationToken, _CallBack, _LifeSystemExepction, _ShowDamageText);
        public static void Damage(Collider collided, List<Collider> _HitTarget, DamageData _Damage, Vector2 _KnockBackPosi, float _KnockForce, float _KnockDuration, float _TimeToEnemyStayInList, CancellationToken _CancellationToken, Action<ITakeDamage, DamageData> _CallBack = null, ITakeDamage _LifeSystemExepction = null, bool _ShowDamageText = true)
        {

            Profiler.BeginSample("DamageExtension - Damage");
            if (collided == null) {
                Profiler.EndSample(); return;
            }
            if (_HitTarget != null && _HitTarget.Contains(collided)) {
                Profiler.EndSample(); return;
            }
            if (!collided.TryGetComponent(out ITakeDamage _TakeDamage)) {
                Profiler.EndSample(); return;
            }
            if (_LifeSystemExepction == _TakeDamage) {
                Profiler.EndSample(); return;
            }

            if (!_TakeDamage.TakeDamage(_Damage, _ShowDamageText)) {
                Profiler.EndSample(); return;
            }
            Profiler.EndSample();

            Profiler.BeginSample("DamageExtension - CallBack");
            _CallBack?.Invoke(_TakeDamage, _Damage);
            Profiler.EndSample();

            if (collided.TryGetComponent(out IKnockbackable _Knockbackable)) _Knockbackable.KnockBack(_KnockBackPosi, _KnockForce, _KnockDuration);

            if (_HitTarget != null)
            {
                ForgiveTarget(_HitTarget, collided, _TimeToEnemyStayInList, _CancellationToken).Forget();
            }

        }

        public static async UniTask ForgiveTarget<T>(List<T> _HitTarget, T _Collider2D, float _TimeToForgive, CancellationToken _CancellationToken)
        {

            _HitTarget.Add(_Collider2D);
            if (await UniTask.WaitForSeconds(_TimeToForgive, cancellationToken: _CancellationToken, ignoreTimeScale: false).SuppressCancellationThrow()) return;
            _HitTarget.Remove(_Collider2D);

        }

    }

}