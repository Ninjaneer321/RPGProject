using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Stats;
using RPG.UI;
using Stats;
using UnityEngine;

namespace RPG.Stats
{
    public class Mana : MonoBehaviour, ISaveable
    {
        [SerializeField] float manaRegenRate = 2; //mana per second

        BaseStats baseStats;
        LazyValue<float> mana;
        public ManaBar manaBar;

        private void Awake()
        {
            mana = new LazyValue<float>(GetInitialMana);
            baseStats = GetComponent<BaseStats>();
        }

        private float GetInitialMana()
        {
            return baseStats.GetStat(Stat.Mana);
        }

        private void Start()
        {
            mana.ForceInit();
        }
        private void Update()
        {
            manaBar.valueText.text = string.Format("{0:0}/{1:0}", GetMana(), GetMaxMana());

            if (mana.value > GetMaxMana())
            {
                mana.value = GetMaxMana();
            }

            if (mana.value < GetMaxMana())
            {
                mana.value += manaRegenRate * Time.deltaTime;
            }
        }

        public float GetMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return baseStats.GetStat(Stat.Mana);
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana.value)
            {
                return false;
            }
            mana.value -= manaToUse;
            manaBar.SetMana(mana.value);
            Debug.Log(mana.value);
            return true;
        }
        public float GetFraction()
        {
            return mana.value / baseStats.GetStat(Stat.Mana);
        }
        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }
    }
}

