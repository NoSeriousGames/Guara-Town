using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuySlot : MonoBehaviour
{
    public Image m_Icon;
    public List<TMP_Text> m_Title;
    [HideInInspector] public Shop m_Controller;
    public Button m_ActivedButton;
    public Button m_DeactivedButton;

    public virtual void Initialize()
    {

    }

    public virtual void Click()
    {

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
