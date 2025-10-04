using UnityEngine;

public class WeaponBuySlot : BuySlot
{
    public WeaponScriptable m_Weapon;

    public override void Initialize()
    {
        base.Initialize();
        m_Icon.sprite = m_Weapon.m_Icon;
        m_Title.text = m_Weapon.m_Title;
    }

    public override void Click()
    {
        base.Click();
        m_Controller.ShowWeaponInfo(m_Weapon);
    }

}
