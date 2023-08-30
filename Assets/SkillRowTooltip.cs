using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillRowTooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI toolbarTooltipText = null;
    public void Setup(string toolbarText)
    {
        toolbarTooltipText.text = toolbarText;
    }
}
