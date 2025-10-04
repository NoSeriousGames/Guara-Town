using UnityEngine;

public class WeaponBuySlot : BuySlot
{
    public WeaponScriptable m_Weapon;

    public override void Initialize()
    {
        base.Initialize();
        m_Icon.sprite = m_Weapon.m_Icon;
        m_Title.ForEach((x) => x.text = m_Weapon.m_Title);
    }

    public override void Click()
    {
        base.Click();
        m_Controller.ShowWeaponInfo(this);
    }

    public void Select()
    {
        m_ActivedButton.gameObject.SetActive(true);
        m_DeactivedButton.gameObject.SetActive(false);
    }

    public void Deselect()
    {
        m_ActivedButton.gameObject.SetActive(false);
        m_DeactivedButton.gameObject.SetActive(true);
    }

}
