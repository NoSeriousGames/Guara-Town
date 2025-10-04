using Game.Utilities.Singleton;

namespace GuaraTower.Data.Save {
    public class InventoryManager : Singleton<InventoryManager> {

        private GuaraTowerSaveData m_GuaraTowerSave;
        public GuaraTowerSaveData GuaraTowerSave { get => m_GuaraTowerSave ??= new GuaraTowerSaveData(); }

    }

}