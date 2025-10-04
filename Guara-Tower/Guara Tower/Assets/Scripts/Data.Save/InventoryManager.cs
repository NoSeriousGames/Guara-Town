using Game.Utilities.Singleton;

namespace GuaraTower.Data.Save {
    public class InventoryManager : Singleton<InventoryManager> {

        public GuaraTowerSaveData m_GuaraTowerSave = new GuaraTowerSaveData();
        public GuaraTowerSaveData GuaraTowerSave { get => m_GuaraTowerSave; }

    }

}