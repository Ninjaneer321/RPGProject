using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class PlayerUICanvas : MonoBehaviour
    {
        [SerializeField] GameObject targetCanvas = null;
        [SerializeField] GameObject playerCanvas = null;
        [SerializeField] public GameObject zoneCanvas = null;
        [SerializeField] ManaBar manaBar = null;

        public Slider targetHealthSlider;
        public Slider playerHealthSlider;
        public Slider playerManaSlider;
        public Gradient gradient;
        public Image targetFill;
        public TextMeshProUGUI targetTextMeshPro = null;
        public TextMeshProUGUI targetHealthTextMeshPro = null;
        public TextMeshProUGUI targetLevelTextMeshPro = null;
        public TextMeshProUGUI zoneTextMeshPro;

        private Fighter fighter;
        private Health health;

        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
                fighter = gameObject.transform.parent.GetComponent<Fighter>();
                health = gameObject.transform.parent.GetComponent<Health>();


                playerCanvas.SetActive(true);
                targetCanvas.SetActive(false);
                zoneCanvas.SetActive(false);
                zoneTextMeshPro.text = "";

        }

        // Update is called once per frame
        void Update()
        {
                playerHealthSlider.value = fighter.GetComponent<Health>().GetFraction();
                playerManaSlider.value = fighter.GetComponent<Mana>().GetFraction();

                if (fighter.target != null)
                {
                    targetCanvas.SetActive(true);
                    targetTextMeshPro.text = fighter.target.name;
                    targetLevelTextMeshPro.text = "LVL: " + fighter.target.GetComponent<BaseStats>().GetLevel().ToString();
                    health = fighter.target.GetComponent<Health>();
                    targetHealthSlider.value = health.GetFraction();
                    targetHealthTextMeshPro.text = string.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
                }
                else
                {
                    targetCanvas.SetActive(false);
                }
            
        }

    }
}


