using MigalhaSystem.Extensions;
using static GuaraTower.Data.Save.GuaraTowerSaveData;
using static GuaraTower.Data.Save.GuaraTowerSaveData.Weapon;

namespace GuaraTower.Data.Save {

    public static class SaveDataUtil {

        public static GuaraTowerSaveData SaveData { get => InventoryManager.Instance.GuaraTowerSave; }

    }

    public static class WeaponSaveUtil {

        public static bool HasWeapon(int _WeaponID) => SaveDataUtil.SaveData.m_WeaponList.TryFind((x)=>x.m_WeaponID == _WeaponID, out int _);

        public static int GetUpgradeLevel(int _WeaponID, int _UpgradeID) {

            if (!SaveDataUtil.SaveData.m_WeaponList.TryFind((x)=>x.m_WeaponID == _UpgradeID, out Weapon _Weapon)) return 0;
            if (!_Weapon.m_UpgradeList.TryFind((x)=>x.m_UpgradeID == _UpgradeID, out Upgrades _Upgrades)) return 0;
            return _Upgrades.m_Level;

        }

        public static int AddUpgradeLevel(int _WeaponID, int _UpgradeID) {

            if (!SaveDataUtil.SaveData.m_WeaponList.TryFind((x) => x.m_WeaponID == _UpgradeID, out Weapon _Weapon)) return 0;

            if (!_Weapon.m_UpgradeList.TryFind((x)=>x.m_UpgradeID == _UpgradeID, out Upgrades _Upgrades)) {

                _Upgrades = new Upgrades();
                _Weapon.m_UpgradeList.Add(_Upgrades);

            }

            _Upgrades.m_Level += 1;

            return _Upgrades.m_Level;

        }

    }

}