using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon Data", menuName = "Guara/Weapons/Weapon Data")]
public class WeaponScriptable : ScriptableObject
{
    
    public int m_ID;
    public string m_Title;
    public string m_Description;
    public Sprite m_Icon;

}
