using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] Transform m_WeaponsSlotPivot;
    [SerializeField] List<WeaponScriptable> m_Weapons;
    [SerializeField] GameObject m_WeaponBuySlotPrefab;
    [SerializeField] TMP_Text m_Title;
    [SerializeField] TMP_Text m_Description;
    List<WeaponBuySlot> m_WeaponSlots = new();

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

        ShowWeaponInfo(m_WeaponSlots[0].m_Weapon);

    }

    public void ShowWeaponInfo(WeaponScriptable _Weapon)
    {

        m_Title.text = _Weapon.m_Title;
        m_Description.text = _Weapon.m_Description;

    }

}
