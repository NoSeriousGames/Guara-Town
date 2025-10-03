using GuaraTower.Core.Interface;

namespace GuaraTower.Arena.Observer {

    using System;

    public static class ObserverPlayerTarget {

        public static Action<IPlayerTarget> AddPlayer;
        public static Action<IPlayerTarget> RemovePlayer;
        public static Action OnTargetUpdate;

        public static void CallAddPlayer(IPlayerTarget _PlayerTarget) {

            AddPlayer?.Invoke(_PlayerTarget);

        }

        public static void CallRemovePlayer(IPlayerTarget _PlayerTarget) {

            RemovePlayer?.Invoke(_PlayerTarget);

        }

        public static void CallTargetUpdate() {

            OnTargetUpdate?.Invoke();

        }

    }

}
