using System.Collections;
using System.Collections.Generic;
using GameDevTV.Core.UI.Tooltips;
using UnityEngine;

public class ToolbarTooltipSpawner : TooltipSpawner
{

    [SerializeField] string toolbarText;
    public override bool CanCreateTooltip()
    {
        return true;
    }

    public override void UpdateTooltip(GameObject tooltip)
    {
        var toolbarTooltip = tooltip.GetComponent<ToolbarTooltip>();
        if (!toolbarTooltip) return;

        toolbarTooltip.Setup(toolbarText);

    }
}
