using GameDevTV.Saving;
using UnityEngine;
using System;
using Stats;

namespace RPG.Stats
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public event Action onExperienceGained;

        private void Update()
        {
            //if (Input.GetKey(KeyCode.E))
            //{
            //    GainExperience(Time.deltaTime * 100);
            //}
        }
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperience()
        {
            return experiencePoints;
        }
        public float GetPoints()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }


        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}