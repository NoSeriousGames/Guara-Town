using GuaraTower.Core.Interface;

namespace GuaraTower.Arena.Observer {

    using System;

    public static class ObserverEnemyTarget {

        public static Action<IEnemyTarget> AddEnemy;
        public static Action<IEnemyTarget> RemoveEnemy;
        public static Action OnTargetUpdate;

        public static void CallAddEnemy(IEnemyTarget _EnemyTarget) {

            AddEnemy?.Invoke(_EnemyTarget);

        }

        public static void CallRemoveEnemy(IEnemyTarget _EnemyTarget) {

            RemoveEnemy?.Invoke(_EnemyTarget);

        }

        public static void CallTargetUpdate() {

            OnTargetUpdate?.Invoke();

        }

    }

}
