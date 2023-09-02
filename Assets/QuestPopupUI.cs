using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestPopupUI : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI questNameText = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void QuestObjectivePopupUI(string objectiveString)
        {
            questNameText.text = "Objective Complete: " + objectiveString;
        }
        public void QuestPopupUIAccept(string acceptQuestString)
        {
            questNameText.text = "Quest Accepted: " + acceptQuestString;
        }

        public void QuestPopupUIComplete(string acceptQuestString)
        {
            questNameText.text = "Quest Completed: " + acceptQuestString;
        }

        public void SetGameobjectInactive()
        {
            gameObject.SetActive(false);
        }
    }
}

