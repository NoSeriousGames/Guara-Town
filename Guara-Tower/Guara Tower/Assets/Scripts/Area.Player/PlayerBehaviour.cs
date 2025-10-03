using GuaraTower.Core.Interface;
using UnityEngine;

namespace GuaraTower.Arena.Player {

    public class PlayerBehaviour : MonoBehaviour, IPlayerTarget {

        private ILifeSystem m_LifeSystem;
        public ILifeSystem GetLifeSystem() {
            return m_LifeSystem ??= GetComponent<ILifeSystem>();
        }

        public Transform GetTransform() {
            return transform;
        }

        private void OnEnable() {

            GetLifeSystem()?.Initialize(1);

        }

    }

}
