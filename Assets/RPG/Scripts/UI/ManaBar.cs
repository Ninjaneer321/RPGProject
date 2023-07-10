using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
	public class ManaBar : MonoBehaviour
	{

		public Slider slider;
		public Gradient gradient;
		public Image fill;
		public TextMeshProUGUI valueText;

        public void SetMaxMana(float mana)
		{
			slider.maxValue = mana;
			slider.value = mana;
			valueText.text = mana.ToString();

			fill.color = gradient.Evaluate(1f);
		}

		public void SetMana(float mana)
		{
			slider.value = mana;
			valueText.text = mana.ToString();
			fill.color = gradient.Evaluate(slider.normalizedValue);
		}

	}

}
