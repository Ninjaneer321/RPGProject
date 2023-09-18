using GameDevTV.Saving;
using UnityEngine;
using System;
using Stats;
using RPG.UI;

namespace RPG.Stats
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float totalExperiencePoints = 0;
        [SerializeField] float experiencePointsGainedTowardsNextLevel = 0;
        [SerializeField] PlayerLevelExperiencePopupUI playerLevelExperiencePopupUI;

        public event Action onExperienceGained;

        //Public Setter for ExperienceGainedTowardsNextLevel
        public void ExperienceGainedTowardsNextLevel(float newValue)
        {
            experiencePointsGainedTowardsNextLevel += newValue;
        }
        public void ResetExperienceGainedTowardsNextLevel(float newValue)
        {
            experiencePointsGainedTowardsNextLevel = newValue;
        }

        //Public Getter for ExperiencePointsGainedTowardsNextLevel
        public float GetExperiencePointsGainedTowardsNextLevel
        {
            get { return experiencePointsGainedTowardsNextLevel; }
        }
        public void GainExperience(float experience)
        {
            totalExperiencePoints += experience;
            experiencePointsGainedTowardsNextLevel += experience;
            playerLevelExperiencePopupUI.SetExperiencePopup(experience.ToString());
            playerLevelExperiencePopupUI.gameObject.SetActive(true);
            onExperienceGained();
        }

        public float GetExperience()
        {
            return totalExperiencePoints;
        }

        public object CaptureState()
        {
            return totalExperiencePoints;
        }


        public void RestoreState(object state)
        {
            totalExperiencePoints = (float)state;
        }
    }
}