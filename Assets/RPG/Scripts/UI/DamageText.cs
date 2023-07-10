using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] Text damageText = null;
    public void DestroyText()
    {
        Destroy(gameObject);
    }

    public void SetValue(float amount)
    {
        if (amount == 0)
        {
            damageText.text = "Dodge!";
            return;
        }
        damageText.text = string.Format("{0:0}", amount);
    }
}
