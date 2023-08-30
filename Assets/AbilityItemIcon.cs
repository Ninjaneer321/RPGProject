using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.Abilities;

public class AbilityItemIcon : MonoBehaviour
{
    // CONFIG DATA
    [SerializeField] TextMeshProUGUI textContainer = null;
    [SerializeField] AbilitySlotUI abilitySlotUI = null;


    // PUBLIC

    public void SetAbility(Ability ability)
    {
        var iconImage = GetComponent<Image>();
        if (ability == null)
        {
            iconImage.enabled = false;
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = ability.GetIcon();
        }
        textContainer.text = abilitySlotUI.GetAbility().GetDisplayName();

    }
}
