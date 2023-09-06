using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityItemIconActionSlot : MonoBehaviour
{
    // CONFIG DATA
    [SerializeField] GameObject textContainer = null;
    [SerializeField] TextMeshProUGUI itemNumber = null;

    // PUBLIC

    public void SetItem(Ability item, int number)
    {
        var iconImage = GetComponent<Image>();
        if (item == null)
        {
            iconImage.enabled = false;
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = item.GetIcon();
        }

        if (itemNumber)
        {
            if (number <= 1)
            {
                textContainer.SetActive(false);
            }
            else
            {
                textContainer.SetActive(true);
                itemNumber.text = number.ToString();
            }
        }
    }
}
