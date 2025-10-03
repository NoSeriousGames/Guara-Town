using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] Transform m_WeaponsSlotPivot;
    [SerializeField] List<WeaponScriptable> m_Weapons;
    [SerializeField] GameObject m_WeaponBuySlotPrefab;

    void Start()
    {

        foreach (WeaponScriptable _Weapon in m_Weapons)
        {
            WeaponBuySlot _WeaponSlot = Instantiate(m_WeaponBuySlotPrefab, m_WeaponsSlotPivot).GetComponent<WeaponBuySlot>();
            _WeaponSlot.m_Weapon = _Weapon;
            _WeaponSlot.Initialize();
        }

    }

}
