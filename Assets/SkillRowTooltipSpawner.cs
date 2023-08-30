using System.Collections;
using System.Collections.Generic;
using GameDevTV.Core.UI.Tooltips;
using UnityEngine;

public class SkillRowTooltipSpawner : TooltipSpawner
{

    [SerializeField] string toolbarText;
    [SerializeField] SkillRowUI skillRowUI;
    public override bool CanCreateTooltip()
    {
        return true;
    }

    public override void UpdateTooltip(GameObject tooltip)
    {
        var skillRowTooltip = tooltip.GetComponent<SkillRowTooltip>();
        if (!skillRowTooltip) return;

        toolbarText = skillRowUI.SkillExperienceGainedTowardsLevel().ToString() + " / " + skillRowUI.GetExperienceNeededBetweenLevels().ToString();
        skillRowTooltip.Setup(toolbarText);

    }


}