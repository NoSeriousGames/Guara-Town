using GuaraTower.Core.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon Data", menuName = "Guara/Weapons/Weapon Data")]
public class WeaponScriptable : ScriptableObject
{
    
    public int m_ID;
    public string m_Title;
    public string m_Description;
    public Sprite m_Icon;
    public List<Upgrade> m_Upgrades = new();
    
    public enum UpgradeType
    {
        Damage,
        AttackSpeed,
        Amount,
        Knockback,
        Ricochet,
        Size,
        Range,
        Slow,
        Speed,
        Lifetime,
    }

    [Serializable()]
    public class Upgrade
    {
        public Sprite m_Icon;
        public UpgradeType m_Type;
        public float m_BaseValues;
        public List<float> m_Prices;
    }

}
