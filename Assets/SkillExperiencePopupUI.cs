using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEngine;


namespace RPG.UI
{
    public class SkillExperiencePopupUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI experienceValueText = null;
        [SerializeField] SkillExperience skillExperience = null;


        public void SetExperiencePopup(string experience, Skill skill)
        {
            experienceValueText.text = "+ " + experience + " " + skill + " EXP";
        }

        public void SetGameobjectInactive()
        {
            gameObject.SetActive(false);
        }
    }
}

