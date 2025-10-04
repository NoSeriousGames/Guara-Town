using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static WeaponScriptable;

public class UpgradeSlot : MonoBehaviour
{
    public Image m_Icon;
    public TMP_Text m_Title;
    [HideInInspector] public Shop m_Controller;
    [HideInInspector] public Upgrade m_Upgrade;

    public virtual void Initialize()
    {

        m_Icon.sprite = m_Upgrade.m_Icon;
        m_Title.text = m_Upgrade.m_Type.ToString();

    }

    public virtual void Click()
    {

    }

}
