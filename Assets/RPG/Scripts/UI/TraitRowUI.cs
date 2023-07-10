using RPG.Stats;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] Stat stat;
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] Button minusButton;
        [SerializeField] Button plusButton;

        TraitStore playerTraitStore = null;

        private void Start()
        {
            playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            minusButton.onClick.AddListener(() => Allocate(-1));
            plusButton.onClick.AddListener(() => Allocate(+1));
        }

        private void Update()
        {
            minusButton.interactable = playerTraitStore.CanAssignPoints(stat, -1);
            plusButton.interactable = playerTraitStore.CanAssignPoints(stat, +1);
            valueText.text = playerTraitStore.GetProposedPoints(stat).ToString();
        }

        public void Allocate(int points)
        {
            playerTraitStore.AssignPoints(stat, points);
        }

    }
}