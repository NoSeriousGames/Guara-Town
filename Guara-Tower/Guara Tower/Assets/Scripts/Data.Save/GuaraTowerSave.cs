using System.Collections.Generic;

namespace GuaraTower.Data.Save {

    [System.Serializable]
    public class GuaraTowerSaveData {

        [System.Serializable]
        public class Weapon {

            public int m_WeaponID;

            [System.Serializable]
            public class Upgrades {

                public int m_UpgradeID;
                public int m_Level;

                public Upgrades() { }

            }
            public List<Upgrades> m_UpgradeList = new List<Upgrades>();

        }
        public List<Weapon> m_WeaponList = new List<Weapon>();

    }

}