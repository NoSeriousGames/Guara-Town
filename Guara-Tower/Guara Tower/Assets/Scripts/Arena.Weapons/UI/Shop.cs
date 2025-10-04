using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static WeaponScriptable;

public class Shop : MonoBehaviour
{
    [SerializeField] Transform m_WeaponsSlotPivot;
    [SerializeField] Transform m_UpgradesSlotPivot;
    [SerializeField] GameObject m_Passives;
    [SerializeField] BuySlot m_PassiveButton;
    [SerializeField] List<WeaponScriptable> m_Weapons;
    [SerializeField] GameObject m_WeaponBuySlotPrefab;
    [SerializeField] GameObject m_UpgradeSlotPrefab;
    [SerializeField] TMP_Text m_Title;
    [SerializeField] TMP_Text m_Description;
    List<WeaponBuySlot> m_WeaponSlots = new();
    List<UpgradeSlot> m_UpgradeSlots = new();

    void Start()
    {

        m_WeaponSlots.Clear();
        foreach (WeaponScriptable _Weapon in m_Weapons)
        {
            WeaponBuySlot _WeaponSlot = Instantiate(m_WeaponBuySlotPrefab, m_WeaponsSlotPivot).GetComponent<WeaponBuySlot>();
            _WeaponSlot.m_Controller = this;
            _WeaponSlot.m_Weapon = _Weapon;
            _WeaponSlot.Initialize();
            m_WeaponSlots.Add(_WeaponSlot);
        }

        ShowWeaponInfo(m_WeaponSlots[0]);

    }

    public void ShowWeaponInfo(WeaponBuySlot _Weapon)
    {

        m_UpgradesSlotPivot.gameObject.SetActive(true);
        m_Passives.SetActive(false);
        m_PassiveButton.Deselect();

        m_WeaponSlots.ForEach((x) => x.Deselect());
        _Weapon.Select();
        m_Title.text = _Weapon.m_Weapon.m_Title;
        m_Description.text = _Weapon.m_Weapon.m_Description;

        m_UpgradeSlots.ForEach((x) => Destroy(x.gameObject));
        m_UpgradeSlots.Clear();
        foreach(Upgrade _Upgrade in _Weapon.m_Weapon.m_Upgrades)
        {
            UpgradeSlot _UpgradeSlot = Instantiate(m_UpgradeSlotPrefab, m_UpgradesSlotPivot).GetComponent<UpgradeSlot>();
            _UpgradeSlot.m_Controller = this;
            _UpgradeSlot.m_Upgrade = _Upgrade;
            _UpgradeSlot.Initialize();
            m_UpgradeSlots.Add(_UpgradeSlot);
        }

    }

    public void ShowPassives()
    {

        m_WeaponSlots.ForEach((x) => x.Deselect());
        m_PassiveButton.Select();
        m_UpgradesSlotPivot.gameObject.SetActive(false);
        m_Passives.SetActive(true);

    }

}
