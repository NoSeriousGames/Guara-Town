using GuaraTower.Core.Interface;
using GuaraTower.Data.Save;
using System.Collections.Generic;
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

        public List<WeaponScriptable> m_WeaponList = new List<WeaponScriptable>();

        private void OnEnable() {

            GetLifeSystem()?.Initialize(1);

            foreach (var weaponData in m_WeaponList) {

                if (!WeaponSaveUtil.HasWeapon(weaponData.m_ID)) continue;
                var controller = Instantiate(weaponData.m_Controller, transform.position, Quaternion.identity);
                controller.GetComponent<IWeaponController>().Initialize();

            }

        }

    }

}
